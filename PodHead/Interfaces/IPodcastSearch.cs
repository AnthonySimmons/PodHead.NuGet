

using System.Collections.Generic;

namespace PodHead.Interfaces
{
    internal interface IPodcastSearch
    {
        IEnumerable<PodcastFeed> Search(string searchTerm, uint maxNumberOfFeeds);
    }
}
