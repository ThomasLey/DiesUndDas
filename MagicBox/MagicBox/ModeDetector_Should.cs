using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace MagicBox
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    internal class ModeDetector_Should
    {
        [Test]
        public void Have_Current_Initialized()
        {
            ModeDetector.Current.Should().NotBeNull();
        }

        [Test]
        public void Not_Have_Debug_And_Release_As_True()
        {
            ModeDetector.Current.IsRelease.Should().Be(!ModeDetector.Current.IsDebug);
            ModeDetector.Current.IsDebug.Should().Be(!ModeDetector.Current.IsRelease);
        }

        [Test]
        [Description("The test above shows that this is not possible with the default implementation. This is not parallelizable because of the static which is changed")]
        [NonParallelizable]
        public void Register_Mock_To_Force_Release_And_Debug()
        {
            var detector = Substitute.For<ModeDetector.IModeDetector>();

            detector.IsDebug.Returns(true);
            detector.IsRelease.Returns(true);
            using (new ModeDetector.TestWith(detector))
            {
                ModeDetector.IsDebug.Should().BeTrue();
                ModeDetector.IsRelease.Should().BeTrue();
            }

            // and not it is back to normal
            ModeDetector.Current.IsRelease.Should().Be(!ModeDetector.Current.IsDebug);

            detector.IsDebug.Returns(false);
            detector.IsRelease.Returns(false);
            using (new ModeDetector.TestWith(detector))
            {
                ModeDetector.IsDebug.Should().BeFalse();
                ModeDetector.IsRelease.Should().BeFalse();
            }

            // and not it is back to normal
            ModeDetector.Current.IsRelease.Should().Be(!ModeDetector.Current.IsDebug);
        }
    }
}