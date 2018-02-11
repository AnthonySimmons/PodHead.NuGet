﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PodHead.FunctionalTests
{
    [TestFixture]
    public class PodHeadFunctionalTests
    {
        private PodHead _podHead;

        private string _errorMessage;

        [SetUp]
        public void SetUp()
        {
            _errorMessage = null;
            _podHead = new PodHead();
            _podHead.ErrorOccurred += PodHead_ErrorOccurred;
        }

        private void PodHead_ErrorOccurred(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        [TearDown]
        public void TearDown()
        {
            _podHead.ErrorOccurred -= PodHead_ErrorOccurred;
            _podHead.Dispose();
        }

        public static IEnumerable<PodcastFeed> PodcastTitlesTestData => new List<PodcastFeed>
        {
            new PodcastFeed("", "", "Monday Morning Podcast", "Bill Burr rants about relationship advice, sports and the Illuminati.", "http://billburr.libsyn.com/rss", "", "", "", "http://billburr.libsyn.com/", "http://is2.mzstatic.com/image/thumb/Music71/v4/ec/3f/92/ec3f924d-e918-086c-777e-cc64e1237984/source/100x100bb.jpg", "Podcasts"),
            new PodcastFeed("", "", "Adam Carolla Show", @"Welcome to the Adam Carolla Show! The new home for the rantings and ravings of Adam CarollThe Adam Carolla Show is the #1 Daily Downloaded Podcast in the World. GET IT ON as Adam shares his thoughts on current events, relationships, airport security, specialty pizzas, politics, and anything else he can complain about. Five days a week and completely uncensored, Adam welcomes a wide range of guests to join him in studio for in depth interviews and a front row seat to his unparalleled ranting. Let's not forget Bryan Bishop (Bald Bryan) on sound effects.

Check it out as Adam hangs out with some of his pals like: Larry Miller, David Allen Grier, Dr. Drew Pinksy, Dana Gould, Doug Benson, and many, many more.", "http://feeds.feedburner.com/TheAdamCarollaPodcast", "", "", "", "http://adamcarolla.com", "http://is5.mzstatic.com/image/thumb/Music127/v4/5d/86/f4/5d86f43c-30a4-8b58-9603-4d32a3ce4bc1/source/100x100bb.jpg", "Podcasts"),
            new PodcastFeed("", "", "The Joe Rogan Experience", "Conduit to the Gaian Mind", "http://joeroganexp.joerogan.libsynpro.com/rss", "", "", "", "http://blog.joerogan.net", "http://is4.mzstatic.com/image/thumb/Music127/v4/d0/e6/5f/d0e65f81-c2cf-7f59-38e4-6abcfab7e38a/source/100x100bb.jpg", "Podcasts"),
        };


        [TestCaseSource(nameof(PodcastTitlesTestData))]
        public void SearchTest(PodcastFeed expectedPodcastFeed)
        {
            IEnumerable<PodcastFeed> podcastFeeds = _podHead.Search(expectedPodcastFeed.Title);
            PodcastFeed feed = podcastFeeds.FirstOrDefault(p => p.Title == expectedPodcastFeed.Title);
            Assert.IsNotNull(feed);
            expectedPodcastFeed.AssertEqual(feed);
            Assert.IsNull(_errorMessage);
        }

        [TestCase(5)]
        [TestCase(10)]
        public void GetTopChartsTest(int limit)
        {
            IEnumerable<PodcastFeed> feeds = _podHead.GetTopCharts(PodcastGenre.Comedy, (uint)limit);
            Assert.AreEqual(limit, feeds.Count());
            Assert.IsNull(_errorMessage);
        }

        [Test, Combinatorial]
        public void LoadPodcastFeedTest([ValueSource(nameof(PodcastTitlesTestData))] PodcastFeed podcastFeed, [Values(5, 10)] int episodeLimit)
        {
            bool result = _podHead.LoadPodcastFeed(podcastFeed, (uint)episodeLimit);
            Assert.IsTrue(result);
            Assert.AreEqual(episodeLimit, podcastFeed.PodcastEpisodes.Count());
            podcastFeed.AssertEpisodes();
            Assert.IsNull(_errorMessage);
        }

        [TestCaseSource(nameof(PodcastTitlesTestData))]
        public void SearchAndLoadTest(PodcastFeed expectedPodcastFeed)
        {
            IEnumerable<PodcastFeed> podcastFeeds = _podHead.Search(expectedPodcastFeed.Title);
            PodcastFeed feed = podcastFeeds.FirstOrDefault(p => p.Title == expectedPodcastFeed.Title);
            Assert.IsNotNull(feed);
            expectedPodcastFeed.AssertEqual(feed);

            foreach (PodcastFeed podcastFeed in podcastFeeds)
            {
                _podHead.LoadPodcastFeed(podcastFeed);
                //Cannot guarentee the result
                podcastFeed.AssertEpisodes();
            }

            Assert.IsNull(_errorMessage);
        }

        [Test]
        public void GetChartsAndLoadTest()
        {
            IEnumerable<PodcastFeed> podcastFeeds = _podHead.GetTopCharts(PodcastGenre.Comedy);
            Assert.AreEqual(10, podcastFeeds.Count());
            PodcastFeed feed = podcastFeeds.First();

            foreach (PodcastFeed podcastFeed in podcastFeeds)
            {
                bool result = _podHead.LoadPodcastFeed(podcastFeed);
                Assert.IsTrue(result, _errorMessage);
                podcastFeed.AssertEpisodes();
            }

            Assert.IsNull(_errorMessage);
        }
    }
}
