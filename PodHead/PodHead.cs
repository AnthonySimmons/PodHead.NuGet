﻿
using PodHead.Interfaces;
using System;
using System.Collections.Generic;

namespace PodHead
{
    public class PodHead : IDisposable
    {
        private readonly IPodcastCharts _podcastCharts;

        private readonly IPodcastSearch _podcastSearch;

        private readonly IRssParser _parser;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event ErrorEventHandler ErrorOccurred;

        private bool _isDisposed;

        public PodHead()
        {
            _parser = new RssParser();
            _podcastCharts = new PodcastCharts(_parser);
            _podcastSearch = new PodcastSearch(_parser);

            _parser.ErrorOccurred += OnError;
            _podcastCharts.ErrorOccurred += OnError;
            _podcastSearch.ErrorOccurred += OnError;
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
        /// Gets a collection <see cref="PodcastFeed"/>, listed at the top charts for the given genre.
        /// </summary>
        /// <param name="genre">Genre for which to read the top charts.</param>
        /// <param name="maxPodcastLimit">Max number of podcasts to read from the top charts.</param>
        /// <returns>Collection of podcast top charts results.</returns>
        public IEnumerable<PodcastFeed> GetTopCharts(PodcastGenre genre, uint maxPodcastLimit = 10)
        {
            return _podcastCharts.GetPodcasts(genre, maxPodcastLimit);
        }

        protected virtual void OnError(string errorMessage)
        {
            ErrorOccurred?.Invoke(errorMessage);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing && !_isDisposed)
            {
                _isDisposed = true;
                _parser.ErrorOccurred -= OnError;
                _podcastCharts.ErrorOccurred -= OnError;
                _podcastSearch.ErrorOccurred -= OnError;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}
