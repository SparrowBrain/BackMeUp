using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BackMeUp.Data;
using BackMeUp.Data.Services;
using BackMeUp.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Data.Services
{
    [TestFixture]
    public class ConfigurationWriterTests
    {
        private class TestableConfigurationWriter : ConfigurationWriter
        {
            public TestableConfigurationWriter(IFile file)
                : base(file)
            {
            }

            protected override Stream GetWriteStream(string configurationXml)
            {
                return StreamToUse;
            }

            public Stream StreamToUse { get; set; }
        }
        private IFile _file;
        private TestableConfigurationWriter GetConfigurationWriter()
        {
            _file = Substitute.For<IFile>();
            return new TestableConfigurationWriter(_file);
        }


        [Test]
        public void Write_Configuration_WritesConfigurationXmlIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var configuration = new Configuration();

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains("<Configuration", configurationXml);
        }

        [Test]
        public void Write_ConfigurationWithSaveGameDirectory_WritesConfigurationXmlWithSaveGameDirectoryIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var configuration = new Configuration
            {
                SaveGamesDirectory = @"C:\Saves",
            };

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains(@"<SaveGamesDirectory>C:\Saves</SaveGamesDirectory>", configurationXml);
        }

        [Test]
        public void Write_ConfigurationWithBackupDirectory_WritesConfigurationXmlWithBackupDirectoryIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var configuration = new Configuration
            {
                BackupDirectory = @"C:\Backup",
            };

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains(@"<BackupDirectory>C:\Backup</BackupDirectory>", configurationXml);
        }

        [Test]
        public void Write_ConfigurationWithBackupPeriod_WritesConfigurationXmlWithBackupPeriodSecondsIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var configuration = new Configuration
            {
                BackupPeriod = TimeSpan.FromMinutes(10),
            };

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains(@"<BackupPeriodSeconds>600</BackupPeriodSeconds>", configurationXml);
        }

        [Test]
        public void Write_ConfigurationWithGameList_WritesConfigurationXmlWithGameListIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var configuration = new Configuration
            {
                GameList = new List<Game> { new Game(12), new Game("Far Cry 3", 46) }
            };

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains(@"<GameList>", configurationXml);
            StringAssert.Contains(@"Far Cry 3", configurationXml);
        }

        [TearDown]
        public void TearDown()
        {
            _file = null;
        }
    }
}