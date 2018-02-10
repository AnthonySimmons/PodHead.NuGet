

using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace PodHead.UnitTest
{
    [TestFixture]
    public class PodHeadTests
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
        public void Search_Should(PodHead sut)
        {
            sut.Search("blah", 25);
        }

    }
}
