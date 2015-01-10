using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services
{
    public class BackupCreator
    {
        private readonly string _backupDirectory;
        private IFileSystem _fileSystem;
        private IBackupDirectoryResolver _backupDirectoryResolver;

        protected IFileSystem FileSystem
        {
            get { return _fileSystem ?? (_fileSystem = GetFileSystem()); }
            set { _fileSystem = value; }
        }

        protected IBackupDirectoryResolver BackupDirectoryResolver
        {
            get { return _backupDirectoryResolver ?? (_backupDirectoryResolver = GetBackupDirectoryResolver()); }
            set { _backupDirectoryResolver = value; }
        }

        public BackupCreator(string backupDirectory)
        {
            _backupDirectory = backupDirectory;
        }

        public BackupCreator(Configuration configuration): this(configuration.BackupDirectory)
        { }

        protected virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }

        protected virtual IBackupDirectoryResolver GetBackupDirectoryResolver()
        {
            return new BackupDirectoryResolver(_backupDirectory);
        }

        public void CreateBackup(string savegame, string gameName)
        {
            FileSystem.CreateDirectoryIfNotExists(_backupDirectory);

            var newTimedBackupPath = BackupDirectoryResolver.GetTimedGameBackupDirectory(gameName);
            var backupPath = BackupDirectoryResolver.GetFullNewBackupDirectory(savegame, newTimedBackupPath);

            FileSystem.CopyDirectory(savegame, backupPath);
        }
    }

}
