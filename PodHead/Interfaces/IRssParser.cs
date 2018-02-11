


namespace PodHead.Interfaces
{
    internal interface IRssParser
    {
        event SubscriptionParsedCompleteEventHandler SubscriptionParsedComplete;
        event ErrorEventHandler ErrorOccurred;

        bool LoadPodcastFeedAsync(PodcastFeed sourceSub, uint limit);
        bool LoadPodcastFeed(PodcastFeed podcastFeed, uint maxEpisodeLimit);
        bool LoadSubscriptionAsync(PodcastFeed sub);
    }
}
