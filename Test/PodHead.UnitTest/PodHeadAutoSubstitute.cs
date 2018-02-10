

using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;
using NSubstitute;
using PodHead.Interfaces;

namespace PodHead.UnitTest
{
    internal class PodHeadAutoSubstitute : AutoDataAttribute
    {
        public PodHeadAutoSubstitute()
            : base(CreateFixture)
        {
        }

        public static IFixture CreateFixture()
        {
            IFixture fixture = new Fixture().Customize(new AutoConfiguredNSubstituteCustomization());
            fixture.Inject(Substitute.For<IPodcastSearch>());
            fixture.Inject(Substitute.For<IPodcastCharts>());
            fixture.Inject(Substitute.For<IRssParser>());
            return fixture;
        }
    }
}
