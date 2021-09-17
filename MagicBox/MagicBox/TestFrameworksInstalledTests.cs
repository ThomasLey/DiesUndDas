using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace MagicBox
{
    [TestFixture]
    public class TestFrameworksInstalledTests
    {
        // NUnit
        [Test]
        public void Test_Packages_Are_Installed()
        {
            // NSubstitute
            var avengers = Substitute.For<IList<string>>();
            avengers.Remove(Arg.Any<string>()).Returns(true);

            var actual = avengers.Remove(@"Spiderman");

            // FluentAssertions
            actual.Should().BeTrue();
        }
    }
}