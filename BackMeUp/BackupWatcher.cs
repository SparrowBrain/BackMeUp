using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Utils;

namespace BackMeUp
{
    public class BackupWatcher
    {
        private readonly string _backupDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeProgramFilesDirectory;
        private IBackupDirectoryResolver _backupDirectoryResolver;
        
        public BackupWatcher(Configuration configuration)
        {
            _backupDirectory = configuration.BackupDirectory;
            _programFilesDirectory = configuration.ProgramFilesDirectory;
            _relativeProgramFilesDirectory = configuration.RelativeProgramFilesLocation;
        }
        
        protected IBackupDirectoryResolver BackupDirectoryResolver
        {
            get { return _backupDirectoryResolver ?? (_backupDirectoryResolver = GetBackupDirectoryResolver()); }
            set { _backupDirectoryResolver = value; }
        }

        protected virtual IBackupDirectoryResolver GetBackupDirectoryResolver()
        {
            return new ProgramFilesDirectoryResolver(_relativeProgramFilesDirectory, _backupDirectory);
        }

        public string GetLatestGameSaveBackup(string name)
        {
            var gameBackupPath = BackupDirectoryResolver.GetLatest(name);
            if (string.IsNullOrEmpty(gameBackupPath))
            {
                return null;
            }
            
            var saveGameDirectory = Path.Combine(gameBackupPath, "ProgramFiles", _relativeProgramFilesDirectory);
            if (!Directory.Exists(saveGameDirectory))
            {
                return null;
            }

            // TODO This looks bad. What about multiuser environment?
            var userDirecotry = Directory.GetDirectories(saveGameDirectory).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                return null;
            }

            var latestSaveGame = Directory.GetDirectories(userDirecotry).FirstOrDefault();

            return latestSaveGame;
        }
    }
}