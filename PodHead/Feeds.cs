using System;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PodHead
{

    internal class Feeds
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

        private readonly Parser _parser;

        private static object _instanceLock = new object();

        public event EventHandler AllFeedsParsed;

        private static Feeds _instance;

        private readonly ErrorLogger _errorLogger;

        private Item _nowPlaying;
        public Item NowPlaying
        {
            get { return _nowPlaying; }
            set
            {
                _nowPlaying = value;
                _nowPlaying.IsNowPlaying = true;
            }
        }

        public static Feeds Get(Parser parser, IConfig config)
        {
            lock (_instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new Feeds(parser, config);
                }
            }
            return _instance;
        }
        
        private Feeds(Parser parser, IConfig config)
        {
            _config = config;
            _parser = parser;
            _errorLogger = ErrorLogger.Get(_config);
            _parser.SubscriptionParsedComplete += Parser_SubscriptionParsedComplete;
        }
        

        private void Parser_SubscriptionParsedComplete(Subscription subscription)
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

        public int MaxItems = 25;

        public ConcurrentList<Subscription> Subscriptions = new ConcurrentList<Subscription>();

		public List<string> Categories 
		{
			get 
			{
				return Subscriptions.GroupBy (ch => ch.Category).Select (g => g.First ().Category).ToList ();
			}
		}
       
        public List<Subscription> ChannelsByCategory(string category)
        {
            return Subscriptions.Where(ch => ch.Category == category).ToList();
        }

        public List<Item> DownloadedItems
        {
            get
            {
                var downloads = new List<Item>();
                
                foreach(var sub in Subscriptions)
                {
                    downloads.AddRange(sub.Items.Where(it => it.IsDownloaded));
                }
                                
                return downloads.ToList();
            }
        }


        public void RemoveChannel(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                foreach (Subscription sub in Subscriptions)
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

        public void AddChannel(Subscription sub)
        {
            if (!String.IsNullOrEmpty(sub.RssLink) && !ContainsSubscription(sub.Title))
            {
                Subscriptions.Add(sub);
                OnSubscriptionAdded(sub);
            }
        }

        public IEnumerable<Item> GetDownloads()
        {
            var downloads = new List<Item>();
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
            foreach (Subscription sub in Subscriptions)
            {
                _parser.LoadSubscription(sub, MaxItems);

                //Do the increment before the calculation.
                OnFeedUpdated((double)++count / (double)Subscriptions.Count);
            }
            OnAllFeedsParsed();

        }
        

        int subsParsed = 0;
        
        private void OnSubscriptionRemoved(Subscription subscription)
        {
            var copy = SubscriptionRemoved;
            if(copy != null)
            {
                copy.Invoke(subscription);
            }
        }

        private void OnSubscriptionAdded(Subscription subscription)
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
            var sub = e.Argument as Subscription;
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
            foreach (Subscription ch in Subscriptions)
            {
                if (ch.Title == title)
                {
                    ch.Feed = feed;
                }
            }
        }

        public void ToggleSubscription(Subscription subscription)
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

        public Subscription GetSubscriptionFromItem(string itemTitle)
        {
            return Subscriptions.FirstOrDefault(sub => sub.Items.Any(it => it.Title == itemTitle));
        }

        public Item GetItem(string title)
        {
            Subscription ch = Subscriptions.FirstOrDefault(m => m.Items.Any(p => p.Title == title));
            Item it = null;
            if (ch != null)
            {
                it = ch.Items.FirstOrDefault(m => m.Title == title);
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
                
                foreach(Subscription sub in Subscriptions.ToList())
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

                    foreach(Item it in sub.GetPlayed().ToList())
                    {
                        var playedItemsElement = xmlDocument.CreateElement(PlayedItems);
                        var itElement = xmlDocument.CreateElement(Item);

                        itElement.SetAttribute(Title, it.Title);
                        itElement.SetAttribute(Duration, it.Duration.ToString());
                        itElement.SetAttribute(Position, it.Position.ToString());
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
                        Subscription subscription = new Subscription(_config)
                        {
                            Category = Convert.ToString(subElement[CategoryName].InnerText),
                            Title = Convert.ToString(subElement[SubscriptionName].InnerText),
                            RssLink = Convert.ToString(subElement[SubscriptionUrl].InnerText)
                        };

                        var playedItems = subElement.GetElementsByTagName(Item);
                        foreach(XmlNode item in playedItems)
                        {
                            Item it = new Item(_config)
                            {
                                Title = Convert.ToString(item.Attributes[Title].Value),
                                Duration = Convert.ToInt32(item.Attributes[Duration].Value),
                                Position = Convert.ToInt32(item.Attributes[Position].Value),
                                IsNowPlaying = Convert.ToBoolean(item.Attributes[IsNowPlaying].Value),
                                ParentSubscription = subscription
                            };
                            subscription.Items.Add(it);

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

        public Subscription findChannelName(string name)
        {
            foreach (Subscription ch in Subscriptions)
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
