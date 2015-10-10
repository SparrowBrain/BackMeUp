using System;
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
    public class MainConfigurationWriterTests
    {
        private class TestableMainConfigurationWriter : MainConfigurationWriter
        {
            public TestableMainConfigurationWriter(IFile file)
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
        private TestableMainConfigurationWriter GetConfigurationWriter()
        {
            _file = Substitute.For<IFile>();
            return new TestableMainConfigurationWriter(_file);
        }


        [Test]
        public void Write_Configuration_WritesConfigurationXmlIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var configuration = new MainConfiguration();

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains("<MainConfiguration", configurationXml);
        }

        [Test]
        public void Write_ConfigurationWithSaveGameDirectory_WritesConfigurationXmlWithSaveGameDirectoryIntoCurrentStream()
        {
            var configurationService = GetConfigurationWriter();
            var memoryStream = new MemoryStream();
            configurationService.StreamToUse = memoryStream;
            var configuration = new MainConfiguration
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
            var configuration = new MainConfiguration
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
            var configuration = new MainConfiguration
            {
                BackupPeriod = TimeSpan.FromMinutes(10),
            };

            configurationService.Write("configuration.xml", configuration);

            var configurationXml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            StringAssert.Contains(@"<BackupPeriodSeconds>600</BackupPeriodSeconds>", configurationXml);
        }

        

        [TearDown]
        public void TearDown()
        {
            _file = null;
        }
    }
}