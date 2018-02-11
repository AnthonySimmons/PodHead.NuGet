

using System.Collections.Generic;

namespace PodHead.Interfaces
{
    internal interface IPodcastCharts
    {
        event ErrorEventHandler ErrorOccurred;

        IEnumerable<PodcastFeed> GetPodcasts(PodcastGenre genre, uint maxPodcastLimit);
    }
}
