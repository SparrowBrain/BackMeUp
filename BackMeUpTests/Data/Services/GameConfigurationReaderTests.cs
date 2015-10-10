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
    public class GameConfigurationReaderTests
    {
        private IFile _file;

        private static readonly string ConfigurationToRead =
            @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
            @"<GameConfiguration xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" +
            Environment.NewLine +
            @"  <Games>" + Environment.NewLine +
            @"    <Game>" + Environment.NewLine +
            @"      <Name>12_Unidentified</Name>" + Environment.NewLine +
            @"      <SaveGameNumber>12</SaveGameNumber>" + Environment.NewLine +
            @"    </Game>" + Environment.NewLine +
            @"    <Game>" + Environment.NewLine +
            @"      <Name>Far Cry 3</Name>" + Environment.NewLine +
            @"      <SaveGameNumber>46</SaveGameNumber>" + Environment.NewLine +
            @"    </Game>" + Environment.NewLine +
            @"  </Games>" + Environment.NewLine +
            @"</GameConfiguration>";

        private readonly byte[] _configurationToReadBytes = Encoding.UTF8.GetBytes(ConfigurationToRead);

        private TestableGameConfigurationReader GetConfigurationReader()
        {
            _file = Substitute.For<IFile>();
            return new TestableGameConfigurationReader(_file);
        }

        private class TestableGameConfigurationReader : GameConfigurationReader
        {
            public TestableGameConfigurationReader(IFile file) : base(file)
            {
            }

            protected override Stream GetReadStream(string configurationXml)
            {
                return StreamToUse;
            }

            public Stream StreamToUse { get; set; }
        }


        [Test]
        public void Read_FileContainsGameList_ConfigurationWithGameList()
        {
            var configurationService = GetConfigurationReader();
            _file.Exists("configuration.xml").Returns(true);
            var memoryStream = new MemoryStream(_configurationToReadBytes);
            configurationService.StreamToUse = memoryStream;

            var configuration = configurationService.Read("configuration.xml");

            var expectedGameList = new List<Game> { new Game(12), new Game("Far Cry 3", 46) };
            Assert.AreEqual(expectedGameList, configuration.Games);
        }

        [TearDown]
        public void TearDown()
        {
            _file = null;
        }
    }
}