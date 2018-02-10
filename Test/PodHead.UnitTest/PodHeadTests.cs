

using NSubstitute;
using NUnit.Framework;
using ObjectExtensions;
using PodHead.Interfaces;
using System;
using System.Collections.Generic;

namespace PodHead.UnitTest
{
    [TestFixture]
    internal class PodHeadTests
    {
        public static IEnumerable<TestCaseData> BadCtorArgsTestData
        {
            get
            {
                yield return new TestCaseData(null,             "appDataFolder", "appDataImageFolder", "configFileName");
                yield return new TestCaseData("downloadFolder", null,            "appDataImageFolder", "configFileName");
                yield return new TestCaseData("downloadFolder", "appDataFolder", null,                 "configFileName");
                yield return new TestCaseData("downloadFolder", "appDataFolder", "appDataImageFolder", null);
            }
        }

        [TestCaseSource(nameof(BadCtorArgsTestData))]
        public void Ctor_ShouldThrowForNullOrEmptyStrings(string downloadFolder,
                                                          string appDataFolder,
                                                          string appDataImageFolder,
                                                          string configFileName)
        {
            Assert.Throws<ArgumentException>(() => new PodHead(downloadFolder, appDataFolder, appDataImageFolder, configFileName));
        }

        [Test]
        public void Ctor_ShouldInstantiate_WhenStringsAreValid()
        {
            Assert.IsNotNull(new PodHead("downloadFolder", "appDataFolder", "appDataImageFolder", "configFileName"));
        }

        [Test, PodHeadAutoSubstitute]
        public void Ctor_ShouldInstantiate_WhenConfigIsNotNull(IConfig config)
        {
            Assert.IsNotNull(new PodHead(config));
        }

        [Test, PodHeadAutoSubstitute]
        public void Ctor_ShouldThrow_WhenConfigIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PodHead(null));
        }

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
