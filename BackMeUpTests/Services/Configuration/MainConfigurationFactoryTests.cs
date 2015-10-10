using System;
using System.IO;
using BackMeUp.Data;
using BackMeUp.Services.Configuration;
using BackMeUp.Utils;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Services.Configuration
{
    [TestFixture]
    public class MainConfigurationFactoryTests
    {
        private IConfigurationReader<MainConfiguration> _configurationReader;
        private IUPlayPathResolver _uPlayPathResolver;

        private MainConfigurationFactory GetConfigurationFactory()
        {
            _configurationReader = Substitute.For<IConfigurationReader<MainConfiguration>>();
            _uPlayPathResolver = Substitute.For<IUPlayPathResolver>();
            _uPlayPathResolver.GetUPlayInstallationDirectory()
                .Returns(@"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher");
            var configurationFactory = new TestableConfigurationFactory {ConfigurationReader = _configurationReader, UPlayPathResolver = _uPlayPathResolver};
            return configurationFactory;
        }

        private class TestableConfigurationFactory : MainConfigurationFactory
        {
            public IConfigurationReader<MainConfiguration> ConfigurationReader { get; set; }
            public IUPlayPathResolver UPlayPathResolver { get; set; }

            protected override IConfigurationReader<MainConfiguration> GetConfigurationReader()
            {
                return ConfigurationReader;
            }

            protected override IUPlayPathResolver GetUPlayPathResolver()
            {
                return UPlayPathResolver;
            }
        }

        [Test]
        public void GetConfiguration_ExceptionWhileReadingFile_ReturnsNewDefaultConfiguration()
        {
            var configurationFactory = GetConfigurationFactory();
            _configurationReader.Read("").ReturnsForAnyArgs(x => { throw new FileNotFoundException(); });
            var defaultConfiguration = new MainConfiguration()
            {
                BackupPeriod = new TimeSpan(0, 10, 0),
                SaveGamesDirectory = @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\savegames",
                BackupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SparrowBrain\\BackMeUp\\Backup")
            };

            var configuration = configurationFactory.GetConfiguration();


            Assert.AreEqual(defaultConfiguration, configuration);
        }

        [Test]
        public void GetConfiguration_ReadsConfiguration_ReturnsConfiguration()
        {
            var configurationFactory = GetConfigurationFactory();
            var expectedConfiguration = new MainConfiguration {BackupDirectory = "C:\\A", SaveGamesDirectory = "C:\\B"};
            _configurationReader.Read("").ReturnsForAnyArgs(expectedConfiguration);

            var configuration = configurationFactory.GetConfiguration();

            Assert.AreEqual(expectedConfiguration, configuration);
        }

        [TearDown]
        public void TeadDown()
        {
            _configurationReader = null;
            _uPlayPathResolver = null;
        }
    }
}
