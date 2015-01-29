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
    public class ConfigurationServiceTests
    {
        private IFile _file;

        private static readonly string ConfigurationToRead =
            @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
            @"<Configuration xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
            @"  <BackupDirectory>C:\Backup</BackupDirectory>" + Environment.NewLine +
            @"  <SaveGamesDirectory>C:\Saves</SaveGamesDirectory>" + Environment.NewLine +
            @"  <BackupPeriodSeconds>600</BackupPeriodSeconds>" + Environment.NewLine +
            @"  <GameList>" + Environment.NewLine +
            @"    <Game>" + Environment.NewLine +
            @"      <Name>12_Unidentified</Name>" + Environment.NewLine +
            @"      <SaveGameNumber>12</SaveGameNumber>" + Environment.NewLine +
            @"    </Game>" + Environment.NewLine +
            @"    <Game>" + Environment.NewLine +
            @"      <Name>Far Cry 3</Name>" + Environment.NewLine +
            @"      <SaveGameNumber>46</SaveGameNumber>" + Environment.NewLine +
            @"    </Game>" + Environment.NewLine +
            @"  </GameList>" + Environment.NewLine +
            @"</Configuration>";

        private readonly byte[] _configurationToReadBytes = Encoding.UTF8.GetBytes(ConfigurationToRead);
        
        private ConfigurationService GetConfigurationService()
        {
            _file = Substitute.For<IFile>();
            return new ConfigurationService(_file);
        }

        [Test]
        public void SetReadStream_MemoryStream_CurrentReadStreamIsSameMemoryStream()
        {
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();

            configurationService.SetReadStream(memoryStream);

            Assert.AreEqual(memoryStream, configurationService.CurrentReadStream);
        }

        [Test]
        public void ResetReadStream_CurrentReadStreamIsNotNull_CurrentReadStreamIsNull()
        {
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();
            configurationService.SetReadStream(memoryStream);

            configurationService.ResetReadStream();

            Assert.IsNull(configurationService.CurrentReadStream);
        }

        [Test]
        public void SetWriteStream_MemoryStream_CurrentWriteStreamIsSameMemoryStream()
        {
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();

            configurationService.SetWriteStream(memoryStream);

            Assert.AreEqual(memoryStream, configurationService.CurrentWriteStream);
        }

        [Test]
        public void ResetWriteStream_CurrentWriteStreamIsNotNull_CurrentWriteStreamIsNull()
        {
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();
            configurationService.SetWriteStream(memoryStream);

            configurationService.ResetWriteStream();

            Assert.IsNull(configurationService.CurrentWriteStream);
        }
       
        [Test]
        public void Read_FileDoesNotExist_ReturnsNull()
        {
            var configurationService = GetConfigurationService();
            _file.Exists("configuration.xml").Returns(false);

            var configuration = configurationService.Read("configuration.xml");

            Assert.IsNull(configuration);
        }

        [Test]
        public void Read_FileCorrectFormat_ReturnsNotNull()
        {
            var configurationService = GetConfigurationService();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.SetReadStream(memoryStream);

            var configuration = configurationService.Read("configuration.xml");

            Assert.IsNotNull(configuration);
        }

        [Test]
        public void Read_FileContainsBackupDirectory_ConfigurationWithBackupDirectory()
        {
            var configurationService = GetConfigurationService();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.SetReadStream(memoryStream);

            var configuration = configurationService.Read("configuration.xml");

            StringAssert.AreEqualIgnoringCase(@"C:\Backup", configuration.BackupDirectory);
        }

        [Test]
        public void Read_FileContainsSaveGameDirectory_ConfigurationWithSaveGameDirectory()
        {
            var configurationService = GetConfigurationService();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.SetReadStream(memoryStream);

            var configuration = configurationService.Read("configuration.xml");

            StringAssert.AreEqualIgnoringCase(@"C:\Saves", configuration.SaveGamesDirectory);
        }

        [Test]
        public void Read_FileContainsBackupPeriodSeconds_ConfigurationWithBackupPeriod()
        {
            var configurationService = GetConfigurationService();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.SetReadStream(memoryStream);

            var configuration = configurationService.Read("configuration.xml");

            Assert.AreEqual(TimeSpan.FromMinutes(10), configuration.BackupPeriod);
        }

        [Test]
        public void Read_FileContainsGameList_ConfigurationWithGameList()
        {
            var configurationService = GetConfigurationService();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.SetReadStream(memoryStream);

            var configuration = configurationService.Read("configuration.xml");

            var expectedGameList = new List<Game>() {new Game(12), new Game("Far Cry 3", 46)};
            Assert.AreEqual(expectedGameList, configuration.GameList);
        }

        [Test]
        public void Write_Configuration_WritesConfigurationXmlIntoCurrentStream()
        {
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();
            configurationService.SetWriteStream(memoryStream);
            var configuration = new Configuration();

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains("<Configuration", configurationXml);
        }

        [Test]
        public void Write_ConfigurationWithSaveGameDirectory_WritesConfigurationXmlWithSaveGameDirectoryIntoCurrentStream()
        {
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();
            configurationService.SetWriteStream(memoryStream);
            var configuration = new Configuration()
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
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();
            configurationService.SetWriteStream(memoryStream);
            var configuration = new Configuration()
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
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();
            configurationService.SetWriteStream(memoryStream);
            var configuration = new Configuration()
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
            var configurationService = GetConfigurationService();
            var memoryStream = new MemoryStream();
            configurationService.SetWriteStream(memoryStream);
            var configuration = new Configuration()
            {
                GameList = new List<Game>() { new Game(12), new Game("Far Cry 3", 46) }
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
