namespace BackMeUp.Services
{
    public class BackupWatcher
    {
        private readonly IBackupDirectoryResolver _backupDirectoryResolver;
        
        public BackupWatcher(IBackupDirectoryResolver backupDirectoryResolver)
        {
            _backupDirectoryResolver = backupDirectoryResolver;
        }

        public string GetLatestGameSaveBackup(string gameName)
        {
            var latestSaveGameBackup = _backupDirectoryResolver.GetLatestSaveGameBackup(gameName);
            return latestSaveGameBackup;
        }
    }
}