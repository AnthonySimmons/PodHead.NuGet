

using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

namespace PodHead.UnitTest
{
    internal class AutoFixtureSubstituteAttribute : AutoDataAttribute
    {
        public AutoFixtureSubstituteAttribute()
            : base(CreateFixture)
        {
        }

        public static IFixture CreateFixture()
        {
            IFixture fixture = new Fixture().Customize(new AutoConfiguredNSubstituteCustomization());
            return fixture;
        }
    }
}
