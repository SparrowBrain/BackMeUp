using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services
{
    public class BackupCreator
    {
        private readonly string _backupDirectory;
        private readonly IFileSystem _fileSystem;
        private readonly IBackupDirectoryResolver _backupDirectoryResolver;

        public BackupCreator(Configuration configuration, IBackupDirectoryResolver backupDirectoryResolver,
            IFileSystem fileSystem)
        {
            _backupDirectory = configuration.BackupDirectory;
            _backupDirectoryResolver = backupDirectoryResolver;
            _fileSystem = fileSystem;
        }

        public void CreateBackup(string savegame, string gameName)
        {
            _fileSystem.CreateDirectoryIfNotExists(_backupDirectory);

            var newTimedBackupPath = _backupDirectoryResolver.GetTimedGameBackupDirectory(gameName);
            var backupPath = _backupDirectoryResolver.GetFullNewBackupDirectory(savegame, newTimedBackupPath);

            _fileSystem.CopyDirectory(savegame, backupPath);
        }
    }

}
