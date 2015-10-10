using System.Collections.Generic;
using System.IO;
using System.Text;
using BackMeUp.Data;
using BackMeUp.Services.Configuration;
using BackMeUp.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Data.Services
{
    [TestFixture]
    public class GameConfigurationWriterTests
    {
        private class TestableGameConfigurationWriter : GameConfigurationWriter
        {
            public TestableGameConfigurationWriter(IFile file)
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
        private TestableGameConfigurationWriter GetConfigurationWriter()
        {
            _file = Substitute.For<IFile>();
            return new TestableGameConfigurationWriter(_file);
        }

        [Test]
        public void Write_GameList_WritesXmlWithGameListIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var gameConfiguration = new GameConfiguration
            {
                Games = new List<Game> { new Game(12), new Game("Far Cry 3", 46) }
            };

            configurationService.Write("configuration.xml", gameConfiguration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains(@"<Games>", configurationXml);
            StringAssert.Contains(@"Far Cry 3", configurationXml);
        }

        [TearDown]
        public void TearDown()
        {
            _file = null;
        }
    }
}