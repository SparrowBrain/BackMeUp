using System;
using System.IO;
using BackMeUp.Utils;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Utils
{
    [TestFixture]
    public class UPlayPathResolverTests
    {
        private IRegistryReader _registryReader;

        private UPlayPathResolver GetUPlayPathResolver()
        {
            _registryReader = Substitute.For<IRegistryReader>();
            var uPlayPathResolver = new TestableUPlayPathResolver {RegistryReader = _registryReader};
            return uPlayPathResolver;
        }

        private class TestableUPlayPathResolver : UPlayPathResolver
        {
            public IRegistryReader RegistryReader { get; set; }

            protected override IRegistryReader GetRegistryReader()
            {
                return RegistryReader;
            }
        }

        [Test]
        public void GetUPlayInstallationDirectory_RegistryReadingFailure_DefaultEnvironmentVariable()
        {
            var uPlayPathResolver = GetUPlayPathResolver();
            _registryReader.GetValue("", "").ReturnsForAnyArgs(x => null);
            var defaultEnvironmentVariable =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"Ubisoft\Ubisoft Game Launcher");

            var uPlayInstallationDirectory = uPlayPathResolver.GetUPlayInstallationDirectory();

            StringAssert.AreEqualIgnoringCase(defaultEnvironmentVariable, uPlayInstallationDirectory);
        }

        [Test]
        public void GetUPlayInstallationDirectory_RegistryReadingSuccess_RegistryValue()
        {
            var uPlayPathResolver = GetUPlayPathResolver();
            _registryReader.GetValue("", "").ReturnsForAnyArgs(@"C:\uPlay\");

            var uPlayInstallationDirectory = uPlayPathResolver.GetUPlayInstallationDirectory();

            StringAssert.AreEqualIgnoringCase(@"D:\uPlay\", uPlayInstallationDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            _registryReader = null;
        }
    }
}
