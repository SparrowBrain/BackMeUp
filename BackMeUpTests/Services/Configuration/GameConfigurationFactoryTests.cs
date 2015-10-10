using System.Collections.Generic;
using System.IO;
using BackMeUp.Data;
using BackMeUp.Services.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Services.Configuration
{
    [TestFixture]
    public class GameConfigurationFactoryTests
    {
        private IConfigurationReader<GameConfiguration> _configurationReader;

        private GameConfigurationFactory GetConfigurationFactory()
        {
            _configurationReader = Substitute.For<IConfigurationReader<GameConfiguration>>();
            var configurationFactory = new TestableConfigurationFactory { ConfigurationReader = _configurationReader };
            return configurationFactory;
        }

        private class TestableConfigurationFactory : GameConfigurationFactory
        {
            public IConfigurationReader<GameConfiguration> ConfigurationReader { get; set; }

            protected override IConfigurationReader<GameConfiguration> GetConfigurationReader()
            {
                return ConfigurationReader;
            }
        }

        [Test]
        public void GetConfiguration_ExceptionWhileReadingFile_ReturnsNewConfiguration()
        {
            var configurationFactory = GetConfigurationFactory();
            _configurationReader.Read("").ReturnsForAnyArgs(x => { throw new FileNotFoundException(); });

            var configuration = configurationFactory.GetConfiguration();

            Assert.AreEqual(new GameConfiguration(), configuration);
        }

        [Test]
        public void GetConfiguration_ReadsConfiguration_ReturnsConfiguration()
        {
            var configurationFactory = GetConfigurationFactory();
            var expectedConfiguration = new GameConfiguration {Games = new List<Game> {new Game(12)}};
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