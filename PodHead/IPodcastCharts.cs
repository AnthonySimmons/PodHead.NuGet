using System.Collections.Generic;

namespace PodHead
{
    internal interface IPodcastCharts
    {
        IEnumerable<PodcastFeed> GetPodcasts(PodcastGenre genre, uint maxPodcastLimit);
    }
}