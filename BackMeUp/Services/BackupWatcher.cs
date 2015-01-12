using NLog;

namespace BackMeUp.Services
{
    public class BackupWatcher
    {
        private readonly IBackupDirectoryResolver _backupDirectoryResolver;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public BackupWatcher(IBackupDirectoryResolver backupDirectoryResolver)
        {
            _backupDirectoryResolver = backupDirectoryResolver;
        }

        public string GetLatestGameSaveBackup(string gameName)
        {
            var latestSaveGameBackup = _backupDirectoryResolver.GetLatestSaveFilesBackupPath(gameName);
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("{0}: Latest backup={1}", "GetLatestGameSaveBackup()", latestSaveGameBackup);
            }
            return latestSaveGameBackup;
        }
    }
}