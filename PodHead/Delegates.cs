using System.Collections.Generic;

namespace PodHead
{

    public delegate void DownloadProgressEvent(PodcastEpisode item, double percent);

    public delegate void DownloadCompleteEvent(PodcastEpisode item);

    public delegate void DownloadRemovedEvent(PodcastEpisode item);

    public delegate void PercentPlayedChangeEvent(PodcastEpisode item);

    public delegate void IsPlayingChangedEvent(PodcastEpisode item);

    internal delegate void ErrorFoundEventHandler(string message);

    internal delegate void SubscriptionParsedCompleteEventHandler(PodcastFeed subscription);

    public delegate void FeedUpdatedEventHandler(double updatePercentage);

    internal delegate void SubscriptionModifiedEventHandler(PodcastFeed subscription);

    public delegate void PodcastSourceUpdateEventHandler(double updatePercentage);

    public delegate void ErrorEventHandler(string errorMessage);

    internal delegate void SearchResultsReceivedEventHandler(IList<PodcastFeed> subscriptions);

}
