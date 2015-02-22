﻿using System;
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
    public class ConfigurationReaderTests
    {
        private IFile _file;

        private static readonly string ConfigurationToRead =
            @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
            @"<Configuration xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" +
            Environment.NewLine +
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

        private TestableConfigurationReader GetConfigurationReader()
        {
            _file = Substitute.For<IFile>();
            return new TestableConfigurationReader(_file);
        }

        private class TestableConfigurationReader : ConfigurationReader
        {
            public TestableConfigurationReader(IFile file) : base(file)
            {
            }

            protected override Stream GetReadStream(string configurationXml)
            {
                return StreamToUse;
            }

            public Stream StreamToUse { get; set; }
        }

        [Test]
        public void Read_FileDoesNotExist_ThrowsException()
        {
            var configurationService = GetConfigurationReader();
            _file.Exists("configuration.xml").Returns(false);

            TestDelegate actDelegate = () => configurationService.Read("configuration.xml");

            Assert.Throws<FileNotFoundException>(actDelegate);
        }

        [Test]
        public void Read_FileCorrectFormat_ReturnsNotNull()
        {
            var configurationService = GetConfigurationReader();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.StreamToUse = memoryStream;

            var configuration = configurationService.Read("configuration.xml");

            Assert.IsNotNull(configuration);
        }

        [Test]
        public void Read_FileContainsBackupDirectory_ConfigurationWithBackupDirectory()
        {
            var configurationService = GetConfigurationReader();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.StreamToUse = memoryStream;

            var configuration = configurationService.Read("configuration.xml");

            StringAssert.AreEqualIgnoringCase(@"C:\Backup", configuration.BackupDirectory);
        }

        [Test]
        public void Read_FileContainsSaveGameDirectory_ConfigurationWithSaveGameDirectory()
        {
            var configurationService = GetConfigurationReader();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.StreamToUse = memoryStream;

            var configuration = configurationService.Read("configuration.xml");

            StringAssert.AreEqualIgnoringCase(@"C:\Saves", configuration.SaveGamesDirectory);
        }

        [Test]
        public void Read_FileContainsBackupPeriodSeconds_ConfigurationWithBackupPeriod()
        {
            var configurationService = GetConfigurationReader();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.StreamToUse = memoryStream;

            var configuration = configurationService.Read("configuration.xml");

            Assert.AreEqual(TimeSpan.FromMinutes(10), configuration.BackupPeriod);
        }

        [Test]
        public void Read_FileContainsGameList_ConfigurationWithGameList()
        {
            var configurationService = GetConfigurationReader();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.StreamToUse = memoryStream;

            var configuration = configurationService.Read("configuration.xml");

            var expectedGameList = new List<Game> {new Game(12), new Game("Far Cry 3", 46)};
            Assert.AreEqual(expectedGameList, configuration.GameList);
        }
        
        [TearDown]
        public void TearDown()
        {
            _file = null;
        }
    }
}
