using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Utils;

namespace BackMeUp
{
    public class BackupWatcher
    {
        private readonly string _backupDirectory;
        private IBackupDirectoryResolver _backupDirectoryResolver;
        
        public BackupWatcher(Configuration configuration)
        {
            _backupDirectory = configuration.BackupDirectory;
        }
        
        protected IBackupDirectoryResolver BackupDirectoryResolver
        {
            get { return _backupDirectoryResolver ?? (_backupDirectoryResolver = GetBackupDirectoryResolver()); }
            set { _backupDirectoryResolver = value; }
        }

        protected virtual IBackupDirectoryResolver GetBackupDirectoryResolver()
        {
            return new BackupDirectoryResolver(_backupDirectory);
        }

        public string GetLatestGameSaveBackup(string gameName)
        {
            var latestSaveGameBackup = BackupDirectoryResolver.GetLatestSaveGameBackup(gameName);
            return latestSaveGameBackup;
        }
    }
}