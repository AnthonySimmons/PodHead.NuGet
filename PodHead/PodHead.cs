
using PodHead.Interfaces;
using System;
using System.Collections.Generic;

namespace PodHead
{
    public class PodHead
    {
        private readonly IPodcastCharts _podcastCharts;

        private readonly IPodcastSearch _podcastSearch;

        private readonly IRssParser _parser;

        private bool _isDisposed;

        public PodHead()
        {
            _parser = new RssParser();
            _podcastCharts = new PodcastCharts(_parser);
            _podcastSearch = new PodcastSearch(_parser);
        }


        /// <summary>
        /// Searches for the given term on the iTunes podcast service.
        /// </summary>
        /// <param name="searchTerm">Term to search for.</param>
        /// <param name="maxNumberOfFeeds">Max number of Feeds to include in the results. (Defaults to 10).</param>
        /// <returns>Collection of podcast feed search results.</returns>
        public IEnumerable<PodcastFeed> Search(string searchTerm, uint maxNumberOfFeeds = 10)
        {
            return _podcastSearch.Search(searchTerm, maxNumberOfFeeds);
        }

        /// <summary>
        /// Searches for the given term on the iTunes podcast service.
        /// </summary>
        /// <param name="searchTerm">Term to search for.</param>
        /// <param name="podcastFeeds">Podcast feeds to be returned.</param>
        /// <param name="errorMessage">A description of any errors that occur, null if successful.</param>
        /// <param name="maxNumberOfFeeds">Max number of Feeds to include in the results (Defaults to 10).</param>
        /// <returns>True when successful, false otherwise.</returns>
        public bool TrySearch(string searchTerm, out IEnumerable<PodcastFeed> podcastFeeds, out string errorMessage, uint maxNumberOfFeeds = 10)
        {
            errorMessage = null;
            podcastFeeds = null;
            try
            {
                podcastFeeds = Search(searchTerm, maxNumberOfFeeds);
                return true;
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Gets a collection <see cref="PodcastFeed"/>, listed at the top charts for the given genre.
        /// </summary>
        /// <param name="genre">Genre for which to read the top charts.</param>
        /// <param name="maxPodcastLimit">Max number of podcasts to read from the top charts (Defaults to 10).</param>
        /// <returns>Collection of podcast top charts results.</returns>
        public IEnumerable<PodcastFeed> GetTopCharts(PodcastGenre genre, uint maxPodcastLimit = 10)
        {
            return _podcastCharts.GetPodcasts(genre, maxPodcastLimit);
        }

        /// <summary>
        /// Gets a collection <see cref="PodcastFeed"/>, listed at the top charts for the given genre.
        /// </summary>
        /// <param name="genre">Genre for which to read the top charts.</param>
        /// <param name="podcastFeeds">Collection of podcasts read from the top charts.</param>
        /// <param name="errorMessage">A description of any error that occurred, null if successful.</param>
        /// <param name="maxPodcastLimit">Max numbe of podcasts to read from the top charts (Defaults to 10).</param>
        /// <returns>True when successful, false otherwise.</returns>
        public bool TryGetTopCharts(PodcastGenre genre, out IEnumerable<PodcastFeed> podcastFeeds, out string errorMessage, uint maxPodcastLimit = 10)
        {
            errorMessage = null;
            podcastFeeds = null;
            try
            {
                podcastFeeds = GetTopCharts(genre, maxPodcastLimit);
                return true;
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
