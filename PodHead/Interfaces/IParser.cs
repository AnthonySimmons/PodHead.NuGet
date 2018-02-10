


namespace PodHead.Interfaces
{
    internal interface IParser
    {
        event SubscriptionParsedCompleteEventHandler SubscriptionParsedComplete;

        bool LoadPodcastFeedAsync(PodcastFeed sourceSub, uint limit);
        bool LoadPodcastFeed(PodcastFeed podcastFeed, uint maxEpisodeLimit);
        bool LoadSubscriptionAsync(PodcastFeed sub);
    }
}
