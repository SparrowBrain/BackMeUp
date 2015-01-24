using System;
using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Services
{
    [TestFixture]
    public class BackupCreatorTests
    {
        private BackupCreator GetBackupCreator(out IFileOperationHelper fakeFileOperationsHelper)
        {
            var configuration = new Configuration
            {
                BackupDirectory = @"E:\Backups",
                SaveGamesDirectory = @"C:\Program Files(x86)\Ubisoft\Ubisoft Game Launcher\savegames"
            };
            IBackupDirectoryResolver fakeBackupDirectoryResolver = Substitute.For<IBackupDirectoryResolver>();
            fakeFileOperationsHelper = Substitute.For<IFileOperationHelper>();
            return new BackupCreator(configuration.BackupDirectory, fakeBackupDirectoryResolver, fakeFileOperationsHelper);
        }

        [Test]
        public void CreateBackup_ValidArguments_CopyDirectoryGetsCalled()
        {
            IFileOperationHelper fakeFileOperationsHelper;
            var backupCreator = GetBackupCreator(out fakeFileOperationsHelper);
            const string saveGameFilesPath = @"C:\Program Files(x86)\Ubisoft\Ubisoft Game Launcher\savegames\849-18691-18169\420";
            const string gameName = "Far Cry Baby";
            
            backupCreator.CreateBackup(saveGameFilesPath, gameName);

            fakeFileOperationsHelper.Received().CopyDirectory(saveGameFilesPath, Arg.Any<string>());
        }

        [TestCase(null)]
        [TestCase("")]
        public void CreateBackup_InvalidSaveGameFilesPath_ThrowsArgumentException(string saveGameFilesPath)
        {
            IFileOperationHelper fakeFileOperationsHelper;
            var backupCreator = GetBackupCreator(out fakeFileOperationsHelper);
            const string gameName = "Far Cry Baby";

            TestDelegate actDelegate = ()=> backupCreator.CreateBackup(saveGameFilesPath, gameName);

            Assert.Throws<ArgumentException>(actDelegate);
        }

        [TestCase(null)]
        [TestCase("")]
        public void CreateBackup_InvalidGameName_ThrowsArgumentException(string gameName)
        {
            IFileOperationHelper fakeFileOperationsHelper;
            var backupCreator = GetBackupCreator(out fakeFileOperationsHelper);
            const string saveGameFilesPath = @"C:\Program Files(x86)\Ubisoft\Ubisoft Game Launcher\savegames\849-18691-18169\420";

            TestDelegate actDelegate = () => backupCreator.CreateBackup(saveGameFilesPath, gameName);

            Assert.Throws<ArgumentException>(actDelegate);
        }
    }
}
