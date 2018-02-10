


namespace PodHead.Interfaces
{
    internal interface IRssParser
    {
        event SubscriptionParsedCompleteEventHandler SubscriptionParsedComplete;

        bool LoadPodcastFeedAsync(PodcastFeed sourceSub, uint limit);
        bool LoadPodcastFeed(PodcastFeed podcastFeed, uint maxEpisodeLimit);
        bool LoadSubscriptionAsync(PodcastFeed sub);
    }
}
