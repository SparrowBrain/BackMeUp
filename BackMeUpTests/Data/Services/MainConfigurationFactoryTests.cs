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

        //TODO this is bad. We need to initialize initial settings (IE, backup dir, etc)
        [Test]
        public void GetConfiguration_ExceptionWhileReadingFile_ReturnsNewConfiguration()
        {
            var configurationFactory = GetConfigurationFactory();
            _configurationReader.Read("").ReturnsForAnyArgs(x => { throw new FileNotFoundException(); });

            var configuration = configurationFactory.GetConfiguration();

            Assert.AreEqual(new MainConfiguration(), configuration);
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
