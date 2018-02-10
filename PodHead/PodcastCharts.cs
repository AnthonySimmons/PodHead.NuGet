using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace PodHead
{

    internal class PodcastCharts
    {
        private readonly Parser _parser;
       
        //https://itunes.apple.com/lookup?id=260190086&entity=podcast
        private const string iTunesLookupUrlFormat = "https://itunes.apple.com/lookup?id={0}&entity={1}";

        public const string iTunesPodcastFormatGenre = "https://itunes.apple.com/us/rss/toppodcasts/limit={0}/genre={1}/xml";

        public const string iTunesPodcastFormatAll = "https://itunes.apple.com/us/rss/toppodcasts/limit={0}/xml";

        private const string EntityPodcast = "podcast";
        
        public event PodcastSourceUpdateEventHandler PodcastSourceUpdated;

        public event ErrorEventHandler ErrorEncountered;

        private readonly IConfig _config;

        private readonly ErrorLogger _errorLogger;

        public PodcastCharts(IConfig config, Parser parser)
		{
            _config = config;
            _parser = parser;
            _errorLogger = ErrorLogger.Get(_config);
		}
        

        private static string GetPodcastInfoJson(string podcastId)
        {
            string json = string.Empty;
            string url = string.Format(iTunesLookupUrlFormat, podcastId, EntityPodcast);

            var webClient = new WebClient();
            Stream st = webClient.OpenRead(url);

            using (var sr = new StreamReader(st))
            {
                json = sr.ReadToEnd();
            }

            return json;
        }

        public static List<PodcastFeed> DeserializeFeeds(string json, IConfig config, Parser parser)
        {
            //Ex.
            //https://itunes.apple.com/lookup?id=278981407&entity=podcast
            var subscriptions = new List<PodcastFeed>();
                        
            string feedUrl = string.Empty;
            JToken rootToken = JObject.Parse(json);
            JToken resultsToken = rootToken["results"];

            foreach (var subToken in resultsToken)
            {
                var sub = new PodcastFeed(config);
                sub.RssLink = (string)subToken["feedUrl"];
                sub.Category = "Podcasts";
                sub.Title = (string)subToken["collectionName"];
                sub.ImageUrl = (string)subToken["artworkUrl100"];
                sub.MaxItems = 0;
				parser.LoadSubscriptionAsync (sub);
                
                subscriptions.Add(sub);
            }

            return subscriptions;
        }
        
        private static string GetPodcastId(string itunesPodcastUrl)
        {
            //Ex.
            //https://itunes.apple.com/us/podcast/monday-morning-podcast/id480486345?mt=2&ign-mpt=uo=2
            // /id(\d)+
            string id = string.Empty;

            var match = Regex.Match(itunesPodcastUrl, @"/id(?<ID>(\d)+)");
            id = match.Groups["ID"].Value;

            return id;
        }

        private string GetiTunesSourceUrl(PodcastGenre genre, int limit)
        {
            var url = string.Empty;
            if (genre != 0)
            {
                url = string.Format(iTunesPodcastFormatGenre, limit, (int)genre);
            }
            else
            {
                url = string.Format(iTunesPodcastFormatAll, limit);
            }
            return url;
        }

        private string GetiTunesSourceRss(PodcastGenre genre, int limit)
        {
            var rss = string.Empty;
            var url = GetiTunesSourceUrl(genre, limit);
            var webClient = new WebClient();

            Stream st = webClient.OpenRead(url);

            using (var sr = new StreamReader(st))
            {
                rss = sr.ReadToEnd();
            }

            return rss;
        }

        private PodcastFeed GetiTunesPodcasts(PodcastGenre genre, int limit)
        {
            var url = GetiTunesSourceUrl(genre, limit);

            var sourceSub = new PodcastFeed(_config)
            {
                RssLink = url,
                Title = genre.ToString(),
                Category = "iTunes",
                MaxItems = limit,
            };
            _parser.LoadPodcastFeedAsync(sourceSub, limit);

            return sourceSub;
        }

        public IList<PodcastFeed> GetPodcasts(PodcastGenre genre, int limit)
        {
            try
            {
                var podcastsChart = GetiTunesPodcasts(genre, limit);
                List<PodcastFeed> feeds = new List<PodcastFeed>();
                int count = 0;
                foreach (var podcast in podcastsChart.PodcastEpisodes)
                {
                    var podcastId = GetPodcastId(podcast.Link);
                    var podcastInfoJson = GetPodcastInfoJson(podcastId);
                    var subscriptions = DeserializeFeeds(podcastInfoJson, _config, _parser);
                    var sub = subscriptions.FirstOrDefault();

                    if (sub != null && feeds.FirstOrDefault(p => p.Title == sub.Title) == null)
                    {
                        feeds.Add(sub);
                    }

                    double percent = (double)(++count) / (double)podcastsChart.PodcastEpisodes.Count;
                    OnPodcastSourceUpdated(percent);
                }
                return feeds;
            }
            catch (Exception ex)
            {
                _errorLogger.Log(ex);
                OnErrorEncountered(ex.Message);
                return null;
            }
        }

		private void OnErrorEncountered(string message)
		{
			var copy = ErrorEncountered;
			if (copy != null) 
			{
				copy.Invoke(message);
			}
		}

		private void OnPodcastSourceUpdated(double percentUpdated)
		{
			var copy = PodcastSourceUpdated;
			if (copy != null) 
			{
				copy.Invoke(percentUpdated);
			}
		}

        public void GetPodcastsAsync(PodcastGenre genre, int limit)
        {
            var thread = new Thread(() => GetPodcasts(genre, limit));
            thread.Start();
        }

    }
}
