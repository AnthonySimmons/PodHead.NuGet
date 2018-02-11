


using NUnit.Framework;

namespace PodHead.FunctionalTests
{
    public static class PodcastAsserts
    {
        public static void AssertEqual(this PodcastFeed podcastFeed, PodcastFeed otherPodcastFeed)
        {
            Assert.AreEqual(podcastFeed.Title,       otherPodcastFeed.Title);
            Assert.AreEqual(podcastFeed.RssLink,     otherPodcastFeed.RssLink);
            Assert.AreEqual(podcastFeed.SiteLink,    otherPodcastFeed.SiteLink);
            Assert.AreEqual(podcastFeed.Description, otherPodcastFeed.Description);
            Assert.AreEqual(podcastFeed.Category,    otherPodcastFeed.Category);
            Assert.AreEqual(podcastFeed.Feed,        otherPodcastFeed.Feed);
            Assert.AreEqual(podcastFeed.ImageUrl,    otherPodcastFeed.ImageUrl);
        }

        public static void AssertEpisodes(this PodcastFeed podcastFeed)
        {
            foreach (PodcastEpisode episode in podcastFeed.PodcastEpisodes)
            {
                Assert.IsNotEmpty(episode.Title);
                Assert.IsNotEmpty(episode.Description);
                Assert.IsNotEmpty(episode.Link);
                Assert.IsNotEmpty(episode.PubDate);
            }
        }
    }
}
