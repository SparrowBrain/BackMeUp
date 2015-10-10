using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackMeUp.Data;
using BackMeUp.Data.Services;
using BackMeUp.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Data.Services
{
    [TestFixture]
    public class MainConfigurationFactoryTests
    {
        private IConfigurationReader<MainConfiguration> _configurationReader;

        private MainConfigurationFactory GetConfigurationFactory()
        {
            _configurationReader = Substitute.For<IConfigurationReader<MainConfiguration>>();
            var configurationFactory = new TestableConfigurationFactory {ConfigurationReader = _configurationReader};
            return configurationFactory;
        }

        private class TestableConfigurationFactory : MainConfigurationFactory
        {
            public IConfigurationReader<MainConfiguration> ConfigurationReader { get; set; }

            protected override IConfigurationReader<MainConfiguration> GetConfigurationReader()
            {
                return ConfigurationReader;
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
                BackupDirectory =
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                        @"Ubisoft\Ubisoft Game Launcher", Constants.SaveGames),
                SaveGamesDirectory =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Backup")
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
        }
    }
}
