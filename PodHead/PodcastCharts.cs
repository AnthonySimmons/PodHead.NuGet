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
        public static readonly Dictionary<string, int> PodcastGenreCodes = new Dictionary<string, int> {
            { "All", 0 },
			{ "Arts", 1301 },
			{ "Business", 1321 },
			{ "Comedy", 1303 },
			{ "Education", 1304 },
			{ "Games & Hobbies", 1323 },
			{ "Government & Organizations", 1325 },
			{ "Health", 1307 },
			{ "Kids & Family", 1305 },
			{ "Music", 1310 },
			{ "News & Politics", 1311 },
			{ "Religion & Spirituality", 1314 },
			{ "Science & Medicine", 1315 },
			{ "Society & Culture", 1324 },
			{ "Sports & Recreation", 1316 },
			{ "Technology", 1318 },
			{ "TV & Film", 1309 },
		};
       

        private readonly Parser _parser;

		public ConcurrentList<Subscription> Podcasts; 

        //https://itunes.apple.com/lookup?id=260190086&entity=podcast
        private const string iTunesLookupUrlFormat = "https://itunes.apple.com/lookup?id={0}&entity={1}";

        public const string iTunesPodcastFormatGenre = "https://itunes.apple.com/us/rss/toppodcasts/limit={0}/genre={1}/xml";

        public const string iTunesPodcastFormatAll = "https://itunes.apple.com/us/rss/toppodcasts/limit={0}/xml";

        private const string EntityPodcast = "podcast";

        public int Limit { get; set; }

        public string Genre { get; set; }

        public const int DefaultLimit = 10;

        public event PodcastSourceUpdateEventHandler PodcastSourceUpdated;

        public event ErrorEventHandler ErrorEncountered;

        private readonly IConfig _config;

        private readonly ErrorLogger _errorLogger;

        public PodcastCharts(IConfig config, Parser parser)
		{
			Limit = DefaultLimit;
			Genre = "Comedy";
			Podcasts = new ConcurrentList<Subscription>();
            _config = config;
            _parser = parser;
            _errorLogger = ErrorLogger.Get(_config);
		}

        public void ClearPodcasts()
        {
            Podcasts.Clear();
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

        public static List<Subscription> DeserializeSubscriptions(string json, IConfig config, Parser parser)
        {
            //Ex.
            //https://itunes.apple.com/lookup?id=278981407&entity=podcast
            var subscriptions = new List<Subscription>();
                        
            string feedUrl = string.Empty;
            JToken rootToken = JObject.Parse(json);
            JToken resultsToken = rootToken["results"];

            foreach (var subToken in resultsToken)
            {
                var sub = new Subscription(config);
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

        private string GetiTunesSourceUrl()
        {
            var url = string.Empty;
            if (PodcastGenreCodes[Genre] != 0)
            {
                url = string.Format(iTunesPodcastFormatGenre, Limit, PodcastGenreCodes[Genre]);
            }
            else
            {
                url = string.Format(iTunesPodcastFormatAll, Limit);
            }
            return url;
        }

        private string GetiTunesSourceRss()
        {
            var rss = string.Empty;
            var url = GetiTunesSourceUrl();
            var webClient = new WebClient();

            Stream st = webClient.OpenRead(url);

            using (var sr = new StreamReader(st))
            {
                rss = sr.ReadToEnd();
            }

            return rss;
        }

        private Subscription GetiTunesPodcasts()
        {
            var url = GetiTunesSourceUrl();

            var sourceSub = new Subscription(_config)
            {
                RssLink = url,
                Title = Genre.ToString(),
                Category = "iTunes",
                MaxItems = Limit,
            };
            _parser.LoadSubscription(sourceSub, Limit);

            return sourceSub;
        }

        private void GetPodcasts()
        {
            try
            {
                var podcastsChart = GetiTunesPodcasts();
                
                int count = 0;
                foreach (var podcast in podcastsChart.Items)
                {                    
                    GetPodcastFromItem(podcast);

                    double percent = (double)(++count) / (double)podcastsChart.Items.Count;
                    OnPodcastSourceUpdated(percent);
                }
            }
            catch (Exception ex)
            {
                _errorLogger.Log(ex);
                OnErrorEncountered(ex.Message);
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            double percent = (double)(Podcasts.Count+1) / (double)Limit;
            OnPodcastSourceUpdated(percent);
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

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var item = e.Argument as Item;
            if(item != null)
            {
                GetPodcastFromItem(item);
            }
        }

        private void GetPodcastFromItem(Item podcastItem)
        {
            var podcastId = GetPodcastId(podcastItem.Link);
            var podcastInfoJson = GetPodcastInfoJson(podcastId);
            var subscriptions = DeserializeSubscriptions(podcastInfoJson, _config, _parser);
            var sub = subscriptions.FirstOrDefault();
            
            if (sub != null && Podcasts.FirstOrDefault(p => p.Title == sub.Title) == null)
            {
                Podcasts.Add(sub);
            }

        }

        public void GetPodcastsAsync()
        {
            var thread = new Thread(GetPodcasts);
            thread.Start();
        }

    }
}
