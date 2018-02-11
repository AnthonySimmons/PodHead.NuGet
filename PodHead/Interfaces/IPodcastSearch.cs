

using System.Collections.Generic;

namespace PodHead.Interfaces
{
    internal interface IPodcastSearch
    {
        event ErrorEventHandler ErrorOccurred;

        IEnumerable<PodcastFeed> Search(string searchTerm, uint maxNumberOfFeeds);
    }
}
