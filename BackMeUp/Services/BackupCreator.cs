using BackMeUp.Data;
using BackMeUp.Utils;
using BackMeUp.Wrappers;

namespace BackMeUp.Services
{
    public class BackupCreator
    {
        private readonly string _backupDirectory;
        private readonly IFileOperationsHelper _fileOperationsHelper;
        private readonly IBackupDirectoryResolver _backupDirectoryResolver;

        public BackupCreator(Configuration configuration, IBackupDirectoryResolver backupDirectoryResolver,
            IFileOperationsHelper fileOperationsHelper)
        {
            _backupDirectory = configuration.BackupDirectory;
            _backupDirectoryResolver = backupDirectoryResolver;
            _fileOperationsHelper = fileOperationsHelper;
        }

        public void CreateBackup(string savegame, string gameName)
        {
            _fileOperationsHelper.CreateDirectoryIfNotExists(_backupDirectory);

            var newTimedBackupPath = _backupDirectoryResolver.GetNewTimedGameBackupPath(gameName);
            var backupPath = _backupDirectoryResolver.GetSaveFilesBackupPath(savegame, newTimedBackupPath);

            _fileOperationsHelper.CopyDirectory(savegame, backupPath);
        }
    }

}
