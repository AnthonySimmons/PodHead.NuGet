

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

        [Test, AutoFixtureSubstitute]
        public void Ctor_ShouldInstantiate_WhenConfigIsNotNull(IConfig config)
        {
            Assert.IsNotNull(new PodHead(null));
        }

        [Test, AutoFixtureSubstitute]
        public void Ctor_ShouldThrow_WhenConfigIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PodHead(null));
        }

        [Test, AutoFixtureSubstitute]
        public void Search_ShouldInvokePodcastSearch(PodHead sut, IPodcastSearch podcastSearch, string searchTerm, uint limit)
        {
            sut.SetField(typeof(IPodcastSearch), podcastSearch);
            sut.Search(searchTerm, limit);
            podcastSearch.Received(1).Search(searchTerm, limit);
        }

    }
}
