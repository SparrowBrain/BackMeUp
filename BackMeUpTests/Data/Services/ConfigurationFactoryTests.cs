using System;
using System.Collections.Generic;
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
    public class ConfigurationFactoryTests
    {
        private IConfigurationReader _configurationReader;

        private ConfigurationFactory GetConfigurationFactory()
        {
            _configurationReader = Substitute.For<IConfigurationReader>();
            var configurationFactory = new TestableConfigurationFactory {ConfigurationReader = _configurationReader};
            return configurationFactory;
        }

        private class TestableConfigurationFactory : ConfigurationFactory
        {
            public IConfigurationReader ConfigurationReader { get; set; }

            protected override IConfigurationReader GetConfigurationReader()
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

            Assert.AreEqual(new Configuration(), configuration);
        }

        [Test]
        public void GetConfiguration_ReadsConfiguration_ReturnsConfiguration()
        {
            var configurationFactory = GetConfigurationFactory();
            var expectedConfiguration = new Configuration {BackupDirectory = "C:\\A", SaveGamesDirectory = "C:\\B"};
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
