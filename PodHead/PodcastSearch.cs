using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using PodHead.Interfaces;

namespace PodHead
{
    internal class PodcastSearch : IPodcastSearch
    {
        public event SearchResultsReceivedEventHandler SearchResultReceived;

        public event ErrorEventHandler ErrorOccurred;

        //https://itunes.apple.com/search?&term=bill+burr&media=podcast&entity=podcast&limit=10

        private const string iTunesSearchUrlFormat = @"https://itunes.apple.com/search?&term={0}&media=podcast&entity=podcast&limit={1}";
        
        private readonly IRssParser _parser;
        
        public PodcastSearch(IRssParser parser)
        {
            _parser = parser;
        }

        public void SearchAsync(string searchTerm, uint maxNumberOfFeeds)
        {
            try
            {
                var searchUrl = GetSearchUrl(searchTerm, maxNumberOfFeeds);
                using (var client = new WebClient())
                {
                    client.OpenReadCompleted += Client_OpenReadCompleted;
                    client.OpenReadAsync(new Uri(searchUrl));
                }
            }
            catch (Exception ex)
            {
                OnErrorEncountered(ex.Message);
            }
        }

        public IEnumerable<PodcastFeed> Search(string searchTerm, uint maxNumberOfFeeds)
        {
            try
            {
                var searchUrl = GetSearchUrl(searchTerm, maxNumberOfFeeds);
                using (var client = new WebClient())
                {
                    Stream stream = client.OpenRead(searchUrl);
                    return ParseSearchResults(stream);
                }
            }
            catch (Exception ex)
            {
                OnErrorEncountered(ex.Message);
                return null;
            }
        }

        private void OnErrorEncountered(string message)
		{
            ErrorOccurred?.Invoke(message);
        }

        private void Client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            IList<PodcastFeed> feeds = ParseSearchResults(e.Result);
            OnSearchResultsReceived(feeds);
        }

        private IList<PodcastFeed> ParseSearchResults(Stream stream)
        { 
            var json = string.Empty;
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }

            return PodcastCharts.DeserializeFeeds(json, _parser);
        }

		private void OnSearchResultsReceived(IList<PodcastFeed> subscriptions)
		{
            SearchResultReceived?.Invoke(subscriptions);
        }

        private static string GetSearchUrl(string searchTerm, uint maxNumberOfFeeds)
        {
            return string.Format(iTunesSearchUrlFormat, searchTerm, maxNumberOfFeeds);
        }
    }
}
