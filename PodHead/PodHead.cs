
using System.Collections.Generic;

namespace PodHead
{
    public class PodHead
    {
        private readonly PodcastCharts _podcastCharts;

        private readonly PodcastSearch _podcastSearch;

        private readonly PodcastFeedManager _feedManager;

        private readonly Parser _parser;

        private readonly IConfig _config;

        public PodHead(string downloadFolder, string appDataFolder, string appDataImageFolder, string configFileName)
            : this(new PodHeadConfig(downloadFolder, appDataFolder, appDataImageFolder, configFileName))
        {
        }

        public PodHead(IConfig config)
        {
            _config = config;
            _parser = new Parser(_config);
            _feedManager = new PodcastFeedManager(_config, _parser);
            _podcastCharts = new PodcastCharts(_config, _parser);
            _podcastSearch = new PodcastSearch(_config, _parser);
        }
       
        /// <summary>
        /// Searches for the given term on the iTunes podcast service.
        /// </summary>
        /// <param name="searchTerm">Term to search for.</param>
        /// <param name="maxNumberOfFeeds">Max number of Feeds to include in the results. (Defaults to 10).</param>
        /// <returns>Collection of podcast feed search results.</returns>
        public IEnumerable<PodcastFeed> Search(string searchTerm, int maxNumberOfFeeds = 10)
        {
            return _podcastSearch.Search(searchTerm, maxNumberOfFeeds);
        }
        
        /// <summary>
        /// Gets a collection <see cref="PodcastFeed"/>, listed at the top charts for the given genre.
        /// </summary>
        /// <param name="genre">Genre for which to read the top charts.</param>
        /// <param name="maxPodcastLimit">Max number of podcasts to read from the top charts.</param>
        /// <returns></returns>
        public IEnumerable<PodcastFeed> GetTopCharts(PodcastGenre genre, int maxPodcastLimit = 10)
        {
            return _podcastCharts.GetPodcasts(genre, maxPodcastLimit);
        }
    }
}
