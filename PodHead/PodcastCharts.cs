﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using PodHead.Interfaces;

namespace PodHead
{
    internal class PodcastCharts : IPodcastCharts
    {
        private readonly IRssParser _parser;

        //https://itunes.apple.com/lookup?id=260190086&entity=podcast
        private const string iTunesLookupUrlFormat = "https://itunes.apple.com/lookup?id={0}&entity={1}";

        public const string iTunesPodcastFormatGenre = "https://itunes.apple.com/us/rss/toppodcasts/limit={0}/genre={1}/xml";

        public const string iTunesPodcastFormatAll = "https://itunes.apple.com/us/rss/toppodcasts/limit={0}/xml";

        private const string EntityPodcast = "podcast";

        public event PodcastSourceUpdateEventHandler PodcastSourceUpdated;

        public event ErrorEventHandler ErrorOccurred;

        public PodcastCharts(IRssParser parser)
        {
            _parser = parser;
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

        public static List<PodcastFeed> DeserializeFeeds(string json, IRssParser parser)
        {
            //Ex.
            //https://itunes.apple.com/lookup?id=278981407&entity=podcast
            var subscriptions = new List<PodcastFeed>();

            string feedUrl = string.Empty;
            JToken rootToken = JObject.Parse(json);
            JToken resultsToken = rootToken["results"];

            foreach (var subToken in resultsToken)
            {
                var sub = new PodcastFeed();
                sub.RssLink = (string)subToken["feedUrl"];
                sub.Category = "Podcasts";
                sub.Title = (string)subToken["collectionName"];
                sub.ImageUrl = (string)subToken["artworkUrl100"];
                sub.MaxItems = 0;
                parser.LoadPodcastFeed(sub, sub.MaxItems);

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

        private string GetiTunesSourceUrl(PodcastGenre genre, uint limit)
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

        private string GetiTunesSourceRss(PodcastGenre genre, uint limit)
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

        private PodcastFeed GetiTunesPodcasts(PodcastGenre genre, uint limit)
        {
            var url = GetiTunesSourceUrl(genre, limit);

            var sourceSub = new PodcastFeed()
            {
                RssLink = url,
                Title = genre.ToString(),
                Category = "iTunes",
                MaxItems = limit,
            };
            _parser.LoadPodcastFeed(sourceSub, limit);

            return sourceSub;
        }

        public IEnumerable<PodcastFeed> GetPodcasts(PodcastGenre genre, uint limit)
        {
            var podcastsChart = GetiTunesPodcasts(genre, limit);
            List<PodcastFeed> feeds = new List<PodcastFeed>();
            int count = 0;
            foreach (var podcast in podcastsChart.PodcastEpisodes)
            {
                var podcastId = GetPodcastId(podcast.Link);
                var podcastInfoJson = GetPodcastInfoJson(podcastId);
                var subscriptions = DeserializeFeeds(podcastInfoJson, _parser);
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

        private void OnErrorEncountered(string message)
        {
            var copy = ErrorOccurred;
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

        public void GetPodcastsAsync(PodcastGenre genre, uint limit)
        {
            var thread = new Thread(() => GetPodcasts(genre, limit));
            thread.Start();
        }

    }
}
