using System;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using PodHead.Interfaces;

namespace PodHead
{

    internal class PodcastFeedManager
    {
        private const string PodHead = "PodHead";
        private const string Subscription = "Subscription";
        private const string CategoryName = "CategoryName";
        private const string SubscriptionName = "SubscriptionName";
        private const string SubscriptionUrl = "SubscriptionUrl";
        private const string PlayedItems = "PlayedItems";
        private const string Item = "Item";
        private const string Title = "Title";
        private const string Duration = "Duration";
        private const string Position = "Position";
        private const string IsNowPlaying = "IsNowPlaying";

        private readonly IConfig _config;

        private readonly IParser _parser;

        public event EventHandler AllFeedsParsed;
     
        private readonly ErrorLogger _errorLogger;

        private PodcastEpisode _nowPlaying;
        public PodcastEpisode NowPlaying
        {
            get { return _nowPlaying; }
            set
            {
                _nowPlaying = value;
                _nowPlaying.IsNowPlaying = true;
            }
        }
        
        public PodcastFeedManager(IConfig config, IParser parser)
        {
            _config = config;
            _parser = parser;
            _errorLogger = ErrorLogger.Get(_config);
            _parser.SubscriptionParsedComplete += Parser_SubscriptionParsedComplete;
        }
        

        private void Parser_SubscriptionParsedComplete(PodcastFeed subscription)
        {
            if (ContainsSubscription(subscription.Title))
            {
                //OnFeedUpdated(subsParsed / Subscriptions.Count);
                if (++subsParsed >= Subscriptions.Count - 1)
                {
                    OnAllFeedsParsed();
                }
            }
        }

        public uint MaxItems = 25;

        public ConcurrentList<PodcastFeed> Subscriptions = new ConcurrentList<PodcastFeed>();

		public List<string> Categories 
		{
			get 
			{
				return Subscriptions.GroupBy (ch => ch.Category).Select (g => g.First ().Category).ToList ();
			}
		}
       
        public List<PodcastFeed> ChannelsByCategory(string category)
        {
            return Subscriptions.Where(ch => ch.Category == category).ToList();
        }

        public List<PodcastEpisode> DownloadedItems
        {
            get
            {
                var downloads = new List<PodcastEpisode>();
                
                foreach(var sub in Subscriptions)
                {
                    downloads.AddRange(sub.PodcastEpisodes.Where(it => it.IsDownloaded));
                }
                                
                return downloads.ToList();
            }
        }


        public void RemoveChannel(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                foreach (PodcastFeed sub in Subscriptions)
                {
                    if (sub.Title == name)
                    {
                        Subscriptions.Remove(sub);
                        Save();
                        OnSubscriptionRemoved(sub);
                        break;
                    }
                }
            }
        }

        public void AddChannel(PodcastFeed sub)
        {
            if (!String.IsNullOrEmpty(sub.RssLink) && !ContainsSubscription(sub.Title))
            {
                Subscriptions.Add(sub);
                OnSubscriptionAdded(sub);
            }
        }

        public IEnumerable<PodcastEpisode> GetDownloads()
        {
            var downloads = new List<PodcastEpisode>();
            foreach(var sub in Subscriptions)
            {
                downloads.AddRange(sub.GetDownloads());
            }
            return downloads;
        }

        public void ParseAllFeedsAsync()
        {
            var thread = new Thread(ParseAllFeeds);
            thread.Start();
        }

        public void ParseAllFeeds()
        {
            subsParsed = 0;
            int count = 0;
            foreach (PodcastFeed sub in Subscriptions)
            {
                _parser.LoadPodcastFeedAsync(sub, MaxItems);

                //Do the increment before the calculation.
                OnFeedUpdated((double)++count / (double)Subscriptions.Count);
            }
            OnAllFeedsParsed();

        }
        

        int subsParsed = 0;
        
        private void OnSubscriptionRemoved(PodcastFeed subscription)
        {
            var copy = SubscriptionRemoved;
            if(copy != null)
            {
                copy.Invoke(subscription);
            }
        }

        private void OnSubscriptionAdded(PodcastFeed subscription)
        {
            var copy = SubscriptionAdded;
            if(copy != null)
            {
                copy.Invoke(subscription);
            }
        }

        private void OnFeedUpdated(double percentUpdate)
        {
            var copy = FeedUpdated;
            if(copy != null)
            {
                copy.Invoke(percentUpdate);
            }
        }
        
        private void OnAllFeedsParsed()
        {
            var copy = AllFeedsParsed;
            if(copy != null)
            {
                copy.Invoke(this, null);
            }
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var sub = e.Argument as PodcastFeed;
            if (sub != null)
            {
                _parser.LoadSubscriptionAsync(sub);
            }
        }
        

        
        public event FeedUpdatedEventHandler FeedUpdated;

        public event SubscriptionModifiedEventHandler SubscriptionAdded;

        public event SubscriptionModifiedEventHandler SubscriptionRemoved;

        public void setChannelFeed(string title, string feed)
        {
            foreach (PodcastFeed ch in Subscriptions)
            {
                if (ch.Title == title)
                {
                    ch.Feed = feed;
                }
            }
        }

        public void ToggleSubscription(PodcastFeed subscription)
        {
            if (ContainsSubscription(subscription.Title))
            {
                RemoveChannel(subscription.Title);
            }
            else
            {
                AddChannel(subscription);
            }
        }

        public PodcastFeed GetSubscriptionFromItem(string itemTitle)
        {
            return Subscriptions.FirstOrDefault(sub => sub.PodcastEpisodes.Any(it => it.Title == itemTitle));
        }

        public PodcastEpisode GetItem(string title)
        {
            PodcastFeed ch = Subscriptions.FirstOrDefault(m => m.PodcastEpisodes.Any(p => p.Title == title));
            PodcastEpisode it = null;
            if (ch != null)
            {
                it = ch.PodcastEpisodes.FirstOrDefault(m => m.Title == title);
            }
            return it;
        }


        public bool ContainsSubscription(string subscriptionTitle)
        {
            return Subscriptions.FirstOrDefault(s => s.Title == subscriptionTitle) != null;
        }

        public string GetSubscribeText(string subscriptionTitle)
        {
            var text = "Subscribe";
            if (ContainsSubscription(subscriptionTitle))
            {
                text = "Unsubscribe";
            }
            return text;
        }

        #region Save and Load


        public void Save()
        {
            Save(_config.ConfigFileName);
        }

        public void Save(string fileName)
        {
            try
            {
                string dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                
                var xmlDocument = new XmlDocument();
                xmlDocument.PreserveWhitespace = true;

                var podHeadElement = xmlDocument.CreateElement(PodHead);
                
                foreach(PodcastFeed sub in Subscriptions.ToList())
                {
                    var subElement = xmlDocument.CreateElement(Subscription);

                    var categoryElement = xmlDocument.CreateElement(CategoryName);
                    var subNameElement = xmlDocument.CreateElement(SubscriptionName);
                    var subUrlElement = xmlDocument.CreateElement(SubscriptionUrl);

                    categoryElement.InnerText = sub.Category;
                    subNameElement.InnerText = sub.Title;
                    subUrlElement.InnerText = sub.RssLink;

                    subElement.AppendChild(categoryElement);
                    subElement.AppendChild(subNameElement);
                    subElement.AppendChild(subUrlElement);

                    foreach(PodcastEpisode it in sub.GetPlayed().ToList())
                    {
                        var playedItemsElement = xmlDocument.CreateElement(PlayedItems);
                        var itElement = xmlDocument.CreateElement(Item);

                        itElement.SetAttribute(Title, it.Title);
                        itElement.SetAttribute(Duration, it.DurationMs.ToString());
                        itElement.SetAttribute(Position, it.PositionPlayedMs.ToString());
                        itElement.SetAttribute(IsNowPlaying, it.IsNowPlaying.ToString());
                        playedItemsElement.AppendChild(itElement);
                        subElement.AppendChild(playedItemsElement);
                    }
                        
                    podHeadElement.AppendChild(subElement);
                }

                xmlDocument.AppendChild(podHeadElement);

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t",
                    NewLineChars = Environment.NewLine,
                    NewLineHandling = NewLineHandling.None,
                    Encoding = new UTF8Encoding(),
                };

                using (var writer = XmlWriter.Create(fileName, settings))
                {
                    xmlDocument.Save(writer);
                }
                
            }
            catch (Exception ex)
            {
                _errorLogger.Log(ex);
            }
        }
        

        public void Load()
        {
            Load(_config.ConfigFileName);
        }

        
        public void Load(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    Subscriptions.Clear();

                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);
                    
                    var subscriptionElements = xmlDocument.GetElementsByTagName(Subscription);
                    foreach(XmlElement subElement in subscriptionElements)
                    {
                        PodcastFeed subscription = new PodcastFeed(_config)
                        {
                            Category = Convert.ToString(subElement[CategoryName].InnerText),
                            Title = Convert.ToString(subElement[SubscriptionName].InnerText),
                            RssLink = Convert.ToString(subElement[SubscriptionUrl].InnerText)
                        };

                        var playedItems = subElement.GetElementsByTagName(Item);
                        foreach(XmlNode item in playedItems)
                        {
                            PodcastEpisode it = new PodcastEpisode(_config)
                            {
                                Title = Convert.ToString(item.Attributes[Title].Value),
                                DurationMs = Convert.ToInt32(item.Attributes[Duration].Value),
                                PositionPlayedMs = Convert.ToInt32(item.Attributes[Position].Value),
                                IsNowPlaying = Convert.ToBoolean(item.Attributes[IsNowPlaying].Value),
                                ParentFeed = subscription
                            };
                            subscription.PodcastEpisodes.Add(it);

                            if(it.IsNowPlaying)
                            {
                                NowPlaying = it;
                            }
                        }
                        AddChannel(subscription);
                    }
                    
                }
            }
            catch (Exception e)
            {
                _errorLogger.Log(e);
            }
        }

        #endregion Save and Load

        public PodcastFeed findChannelName(string name)
        {
            foreach (PodcastFeed ch in Subscriptions)
            {
                if (name == ch.Title)
                {
                    return ch;
                }
            }
            return null;
        }

    }
}
