using System;
using BackMeUp.Utils;
using NLog;

namespace BackMeUp.Services
{
    public class BackupCreator
    {
        private readonly string _backupDirectory;
        private readonly IFileOperationHelper _fileOperationsHelper;
        private readonly IBackupDirectoryResolver _backupDirectoryResolver;
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BackupCreator(string backupDirectory, IBackupDirectoryResolver backupDirectoryResolver,
            IFileOperationHelper fileOperationsHelper)
        {
            _backupDirectory = backupDirectory;
            _backupDirectoryResolver = backupDirectoryResolver;
            _fileOperationsHelper = fileOperationsHelper;
        }

        public void CreateBackup(string saveGameFilesPath, string gameName)
        {
            if (string.IsNullOrEmpty(saveGameFilesPath))
            {
                throw new ArgumentException("saveGameFilesPath");
            }
            if (string.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("gameName");
            }

            _fileOperationsHelper.CreateDirectoryIfNotExists(_backupDirectory);

            var newTimedBackupPath = _backupDirectoryResolver.GetNewTimedGameBackupPath(gameName);
            var backupPath = _backupDirectoryResolver.GetSaveFilesBackupPath(saveGameFilesPath, newTimedBackupPath);

            _fileOperationsHelper.CopyDirectory(saveGameFilesPath, backupPath);
            
            if (Logger.IsInfoEnabled)
            {
                Logger.Info("{0}: Backup created={1}", "CreateBackup()", backupPath);
            }
        }
    }
}
