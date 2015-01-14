using System;
using System.IO;
using BackMeUp.Services;
using BackMeUp.Utils;
using BackMeUp.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Services
{
    [TestFixture]
    public class BackupDirectoryResolverTests
    {
        private IBackupDirectoryResolver CreateBackupDirectoryResolver(out IDirectory directory,
            out IDirectoryNameFixer directoryNameFixer)
        {
            directory = Substitute.For<IDirectory>();
            directoryNameFixer = Substitute.For<IDirectoryNameFixer>();

            return new BackupDirectoryResolver(@"E:\Backup", directory, directoryNameFixer);
        }

        [Test]
        public void GetSaveFilesBackupPath_ValidParameters_ReturnsGoodBackupPath()
        {
            const string gameSaveFilesDirectory = @"C:\savegames\87987-189118-191991\1337";
            const string timedBackupPath = @"E:\Backup\Far Cry 3\2015-01-10_212211";
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);

            var saveFilesBackupPath = backupDirectoryResolver.GetSaveFilesBackupPath(gameSaveFilesDirectory, timedBackupPath);

            StringAssert.AreEqualIgnoringCase(
                @"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991\1337", saveFilesBackupPath);
        }

        [TestCase(null)]
        [TestCase("")]
        public void GetSaveFilesBackupPath_InvalidGameSaveFilesDirectory_ThrowsArgumentException(string gameSaveFilesDirectory)
        {
            const string timedBackupPath = @"E:\Backup\Far Cry 3\2015-01-10_212211";
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);

            TestDelegate actDelegate = ()=> backupDirectoryResolver.GetSaveFilesBackupPath(gameSaveFilesDirectory, timedBackupPath);

            Assert.Throws<ArgumentException>(actDelegate);
        }

        [TestCase(null)]
        [TestCase("")]
        public void GetSaveFilesBackupPath_InvalidTimedBackupPath_ThrowsArgumentException(string timedBackupPath)
        {
            const string gameSaveFilesDirectory = @"C:\savegames\87987-189118-191991\1337";
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);

            TestDelegate actDelegate = () => backupDirectoryResolver.GetSaveFilesBackupPath(gameSaveFilesDirectory, timedBackupPath);

            Assert.Throws<ArgumentException>(actDelegate);
        }

        [Test]
        public void GetLatestSaveFilesBackupPath_ValidGameNameBackupExists_LatestBackupPath()
        {
            const string gameName = "Far Cry 3";
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);
            directoryNameFixer.ReplaceInvalidCharacters("").ReturnsForAnyArgs(x => x.Arg<string>());
            directory.Exists("").ReturnsForAnyArgs(true);
            directory.GetDirectories(@"E:\Backup\Far Cry 3")
                .Returns(new[] {@"E:\Backup\Far Cry 3\2015-01-10_212211", @"E:\Backup\Far Cry 3\2010-01-01_000000"});
            directory.GetDirectories(@"E:\Backup\Far Cry 3\2015-01-10_212211\savegames")
                .Returns(new[] { @"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991" });
            directory.GetDirectories(@"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991")
                .Returns(new[] { @"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991\46" });

            var latestSaveFilesBackupPath = backupDirectoryResolver.GetLatestSaveFilesBackupPath(gameName);

            StringAssert.AreEqualIgnoringCase(
                @"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991\46",
                latestSaveFilesBackupPath);
        }

        [Test]
        public void GetLatestSaveFilesBackupPath_GameNameWithInvalidCharactersBackupExists_LatestBackupPath()
        {
            const string gameName = "Far:Cry?";
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);
            directoryNameFixer.ReplaceInvalidCharacters("").ReturnsForAnyArgs("Far_Cry_");
            directory.Exists("").ReturnsForAnyArgs(true);
            directory.GetDirectories(@"E:\Backup\Far_Cry_")
                .Returns(new[] { @"E:\Backup\Far_Cry_\2015-01-10_212211", @"E:\Backup\Far_Cry_\2010-01-01_000000" });
            directory.GetDirectories(@"E:\Backup\Far_Cry_\2015-01-10_212211\savegames")
                .Returns(new[] { @"E:\Backup\Far_Cry_\2015-01-10_212211\savegames\87987-189118-191991" });
            directory.GetDirectories(@"E:\Backup\Far_Cry_\2015-01-10_212211\savegames\87987-189118-191991")
                .Returns(new[] { @"E:\Backup\Far_Cry_\2015-01-10_212211\savegames\87987-189118-191991\46" });

            var latestSaveFilesBackupPath = backupDirectoryResolver.GetLatestSaveFilesBackupPath(gameName);

            StringAssert.AreEqualIgnoringCase(
                @"E:\Backup\Far_Cry_\2015-01-10_212211\savegames\87987-189118-191991\46",
                latestSaveFilesBackupPath);
        }

        [TestCase(@"E:\Backup\Far Cry 3")]
        [TestCase(@"E:\Backup\Far Cry 3\2015-01-10_212211")]
        [TestCase(@"E:\Backup\Far Cry 3\2015-01-10_212211\savegames")]
        [TestCase(@"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991")]
        [TestCase(@"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991\46")]
        public void GetLatestSaveFilesBackupPath_GameBackupPathElementDoesNotExist_Null(string notExistingDirectory)
        {
            const string gameName = "Far Cry 3";
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);
            directoryNameFixer.ReplaceInvalidCharacters("").ReturnsForAnyArgs(x => x.Arg<string>());
            directory.Exists("").ReturnsForAnyArgs(true);
            directory.GetDirectories(@"E:\Backup\Far Cry 3")
                .Returns(new[] { @"E:\Backup\Far Cry 3\2015-01-10_212211", @"E:\Backup\Far Cry 3\2010-01-01_000000" });
            directory.GetDirectories(@"E:\Backup\Far Cry 3\2015-01-10_212211\savegames")
                .Returns(new[] { @"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991" });
            directory.GetDirectories(@"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991")
                .Returns(new[] { @"E:\Backup\Far Cry 3\2015-01-10_212211\savegames\87987-189118-191991\46" });

            directory.Exists(notExistingDirectory).Returns(false);
            directory.GetDirectories(Directory.GetParent(notExistingDirectory).FullName).Returns(new string[0]);

            var latestSaveFilesBackupPath = backupDirectoryResolver.GetLatestSaveFilesBackupPath(gameName);

            Assert.IsNull(latestSaveFilesBackupPath);
        }

        [TestCase(null)]
        [TestCase("")]
        public void GetLatestSaveFilesBackupPath_GameNameNullOrEmpty_ThrowsArgumentException(string gameName)
        {
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);

            TestDelegate actDelegate = ()=> backupDirectoryResolver.GetLatestSaveFilesBackupPath(gameName);

            Assert.Throws<ArgumentException>(actDelegate);
        }

        [TestCase("Far Cry 3", @"E:\Backup\Far Cry 3\2015-01-10_212211")]
        [TestCase("Far:Cry?", @"E:\Backup\Far_Cry_\2015-01-10_212211")]
        public void GetNewTimedGameBackupPath_ValidGameName_ValidBackupPathWithFixedName(string gameName, string expectedPath)
        {
            SystemTime.SetDateTime(new DateTime(2015, 01, 10, 21, 22, 11));
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);
            directoryNameFixer.ReplaceInvalidCharacters("").ReturnsForAnyArgs(x => x.Arg<string>());
            directoryNameFixer.ReplaceInvalidCharacters("Far:Cry?").Returns("Far_Cry_");

            var newTimedGameBackupPath = backupDirectoryResolver.GetNewTimedGameBackupPath(gameName);

            StringAssert.AreEqualIgnoringCase(
                expectedPath,
                newTimedGameBackupPath);
        }

        [TestCase(null)]
        [TestCase("")]
        public void GetNewTimedGameBackupPath_GameNameNullOrEmpty_ThrowsArgumentException(string gameName)
        {
            IDirectory directory;
            IDirectoryNameFixer directoryNameFixer;
            var backupDirectoryResolver = CreateBackupDirectoryResolver(out directory, out directoryNameFixer);

            TestDelegate actDelegate = ()=> backupDirectoryResolver.GetNewTimedGameBackupPath(gameName);

            Assert.Throws<ArgumentException>(actDelegate);
        }

        [TearDown]
        public void TeadDown()
        {
            SystemTime.Reset();
        }
    }
}
