using System;
using NLog;

namespace BackMeUp.Services
{
    public class BackupWatcher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IBackupDirectoryResolver _backupDirectoryResolver;

        public BackupWatcher(IBackupDirectoryResolver backupDirectoryResolver)
        {
            _backupDirectoryResolver = backupDirectoryResolver;
        }

        public string GetLatestGameSaveBackup(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("gameName");
            }

            var latestSaveGameBackup = _backupDirectoryResolver.GetLatestSaveFilesBackupPath(gameName);
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("{0}: Latest backup={1}", "GetLatestGameSaveBackup()", latestSaveGameBackup);
            }
            return latestSaveGameBackup;
        }
    }
}