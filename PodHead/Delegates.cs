using System.Collections.Generic;

namespace PodHead
{

    internal delegate void DownloadProgressEvent(Item item, double percent);

    internal delegate void DownloadCompleteEvent(Item item);

    internal delegate void DownloadRemovedEvent(Item item);

    internal delegate void PercentPlayedChangeEvent(Item item);

    internal delegate void IsPlayingChangedEvent(Item item);

    internal delegate void ErrorFoundEventHandler(string message);

    internal delegate void SubscriptionParsedCompleteEventHandler(Subscription subscription);

    public delegate void FeedUpdatedEventHandler(double updatePercentage);

    internal delegate void SubscriptionModifiedEventHandler(Subscription subscription);

    public delegate void PodcastSourceUpdateEventHandler(double updatePercentage);

    public delegate void ErrorEventHandler(string errorMessage);

    internal delegate void SearchResultsReceivedEventHandler(List<Subscription> subscriptions);

}
