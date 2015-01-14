using System;
using System.IO;
using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Services
{
    [TestFixture]
    public class SaveWatcherTests
    {
        private readonly Configuration _configuration = new Configuration
        {
            BackupDirectory = @"E:\Backups",
            SaveGamesDirectory = @"C:\savegames"
        };

        [Test]
        public void GetLatestSaveFilesPath_MultipleUsersMultipleGames_ReturnsLastWrittenDirectory()
        {
            IDirectory directory = Substitute.For<IDirectory>();
            directory.GetDirectories(@"C:\savegames", Arg.Any<string>(), SearchOption.AllDirectories)
                .Returns(new[]
                {
                    @"C:\savegames\87987-189118-191991",
                    @"C:\savegames\87987-189118-191991\55",
                    @"C:\savegames\87987-189118-191991\1337",
                    @"C:\savegames\99987-222222-191991\",
                    @"C:\savegames\99987-222222-191991\1984"
                });
            directory.GetLastWriteTime(@"C:\savegames\87987-189118-191991\55").Returns(new DateTime(2000, 01, 01));
            directory.GetLastWriteTime(@"C:\savegames\87987-189118-191991\1337").Returns(new DateTime(2013, 01, 01));
            directory.GetLastWriteTime(@"C:\savegames\99987-222222-191991\1984").Returns(new DateTime(2008, 01, 01));
            SaveWatcher saveWatcher = new SaveWatcher(_configuration, directory);

            var latestSaveFilesPath = saveWatcher.GetLatestSaveFilesPath();

            StringAssert.AreEqualIgnoringCase(@"C:\savegames\87987-189118-191991\1337", latestSaveFilesPath);
        }

        [Test]
        public void GetLatestSaveFilesPath_NoUsersExist_ReturnNull()
        {
            IDirectory directory = Substitute.For<IDirectory>();
            directory.GetDirectories(@"C:\savegames", Arg.Any<string>(), SearchOption.AllDirectories)
                .Returns(new string[0]);
            SaveWatcher saveWatcher = new SaveWatcher(_configuration, directory);

            var latestSaveFilesPath = saveWatcher.GetLatestSaveFilesPath();

            Assert.IsNull(latestSaveFilesPath);
        }

        [Test]
        public void GetLatestSaveFilesPath_NoSaveFilesExist_ReturnNull()
        {
            IDirectory directory = Substitute.For<IDirectory>();
            directory.GetDirectories(@"C:\savegames", Arg.Any<string>(), SearchOption.AllDirectories)
                .Returns(new[]
                {
                    @"C:\savegames\87987-189118-191991",
                    @"C:\savegames\99987-222222-191991\",
                });
            SaveWatcher saveWatcher = new SaveWatcher(_configuration, directory);

            var latestSaveFilesPath = saveWatcher.GetLatestSaveFilesPath();

            Assert.IsNull(latestSaveFilesPath);
        }
    }
}
