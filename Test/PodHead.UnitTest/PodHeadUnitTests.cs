

using NSubstitute;
using NUnit.Framework;
using ObjectExtensions;
using PodHead.Interfaces;

namespace PodHead.UnitTest
{
    [TestFixture]
    internal class PodHeadUnitTests
    {
        [Test, PodHeadAutoSubstitute]
        public void Search_ShouldInvokePodcastSearch(PodHead sut, IPodcastSearch podcastSearch, string searchTerm, uint limit)
        {
            sut.SetField(typeof(IPodcastSearch), podcastSearch);
            sut.Search(searchTerm, limit);
            podcastSearch.Received(1).Search(searchTerm, limit);
        }

        [Test, PodHeadAutoSubstitute]
        public void GetTopCharts_ShouldInvokePodcastCharts(PodHead sut, IPodcastCharts podcastCharts, PodcastGenre podcastGenre, uint limit)
        {
            sut.SetField(typeof(IPodcastCharts), podcastCharts);
            sut.GetTopCharts(podcastGenre, limit);
            podcastCharts.Received(1).GetPodcasts(podcastGenre, limit);
        }

        [Test, PodHeadAutoSubstitute]
        public void LoadPodcastFeed_ShouldInvokePodcastFeed(PodHead sut, PodcastFeed podcastFeed, IRssParser rssParser, uint limit)
        {
            sut.SetField(typeof(IRssParser), rssParser);
            sut.LoadPodcastFeed(podcastFeed, limit);
            rssParser.Received(1).LoadPodcastFeed(podcastFeed, limit);
        }        
    }
}
