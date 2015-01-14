using System;
using BackMeUp.Services;
using NSubstitute;
using NUnit.Framework;

namespace BackMeUp.UnitTests.Services
{
    [TestFixture]
    public class BackupWatcherTests
    {
        [TestCase(null)]
        [TestCase("")]
        public void GetLatestGameSaveBackup_InvalidGameName_ThrowsException(string gameName)
        {
            IBackupDirectoryResolver fakeBackupDirectoryResolver = Substitute.For<IBackupDirectoryResolver>();
            BackupWatcher backupWatcher = new BackupWatcher(fakeBackupDirectoryResolver);

            TestDelegate actDelegate = () => backupWatcher.GetLatestGameSaveBackup(gameName);

            Assert.Throws<ArgumentException>(actDelegate);
        }
    }
}