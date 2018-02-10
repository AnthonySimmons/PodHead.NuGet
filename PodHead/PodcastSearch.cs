using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace PodHead
{
        

    internal class PodcastSearch
    {
        public event SearchResultsReceivedEventHandler SearchResultReceived;

        public event ErrorEventHandler ErrorEncountered;

        //https://itunes.apple.com/search?&term=bill+burr&media=podcast&entity=podcast&limit=10

        private const string iTunesSearchUrlFormat = @"https://itunes.apple.com/search?&term={0}&media=podcast&entity=podcast&limit={1}";
        
        private readonly IConfig _config;

        private readonly Parser _parser;

        private readonly ErrorLogger _errorLogger;
        
        public PodcastSearch(IConfig config, Parser parser)
        {
            _config = config;
            _parser = parser;
            _errorLogger = ErrorLogger.Get(_config);
        }

        public void SearchAsync(string searchTerm, int maxNumberOfFeeds)
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
                _errorLogger.Log(ex);
                OnErrorEncountered(ex.Message);
            }
        }

        public IList<PodcastFeed> Search(string searchTerm, int maxNumberOfFeeds)
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
                _errorLogger.Log(ex);
                OnErrorEncountered(ex.Message);
                return null;
            }
        }

        private void OnErrorEncountered(string message)
		{
            ErrorEncountered?.Invoke(message);
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

            return PodcastCharts.DeserializeFeeds(json, _config, _parser);
        }

		private void OnSearchResultsReceived(IList<PodcastFeed> subscriptions)
		{
            SearchResultReceived?.Invoke(subscriptions);
        }

        private static string GetSearchUrl(string searchTerm, int maxNumberOfFeeds)
        {
            return string.Format(iTunesSearchUrlFormat, searchTerm, maxNumberOfFeeds);
        }
    }
}
