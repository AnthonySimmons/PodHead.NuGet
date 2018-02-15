
using NSubstitute;
using NUnit.Framework;
using ObjectExtensions;
using PodHead.Interfaces;
using System.Collections.Generic;

namespace PodHead.UnitTest
{
    [TestFixture]
    internal class PodHeadUnitTests
    {
        [Test, PodHeadAutoSubstitute]
        public void Search_ShouldInvokePodcastSearchUnitTest(PodHead sut, IPodcastSearch podcastSearch, string searchTerm, uint limit)
        {
            sut.SetField(typeof(IPodcastSearch), podcastSearch);
            sut.Search(searchTerm, limit);
            podcastSearch.Received(1).Search(searchTerm, limit);
        }

        [Test, PodHeadAutoSubstitute]
        public void GetTopCharts_ShouldInvokePodcastChartsUnitTest(PodHead sut, IPodcastCharts podcastCharts, PodcastGenre podcastGenre, uint limit)
        {
            sut.SetField(typeof(IPodcastCharts), podcastCharts);
            sut.GetTopCharts(podcastGenre, limit);
            podcastCharts.Received(1).GetPodcasts(podcastGenre, limit);
        }

        [Test, PodHeadAutoSubstitute]
        public void TrySearch_ShouldInvokePodcastSearchUnitTest(PodHead sut, IPodcastSearch podcastSearch, string searchTerm, uint limit)
        {
            sut.SetField(typeof(IPodcastSearch), podcastSearch);
            bool success = sut.TrySearch(searchTerm, out IEnumerable<PodcastFeed> podcasts, out string errors, limit);
            Assert.IsTrue(success);
            Assert.IsNull(errors);
            podcastSearch.Received(1).Search(searchTerm, limit);
        }

        [Test, PodHeadAutoSubstitute]
        public void TryGetTopCharts_ShouldInvokePodcastChartsUnitTest(PodHead sut, IPodcastCharts podcastCharts, PodcastGenre podcastGenre, uint limit)
        {
            sut.SetField(typeof(IPodcastCharts), podcastCharts);
            bool success = sut.TryGetTopCharts(podcastGenre, out IEnumerable<PodcastFeed> podcasts, out string errors, limit);
            Assert.IsTrue(success);
            Assert.IsNull(errors);
            podcastCharts.Received(1).GetPodcasts(podcastGenre, limit);
        }
    }
}
