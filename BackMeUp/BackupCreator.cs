using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackMeUp
{
        //    public BackupCalledWithValidConfiguration()
        //{
        //    _backupDirectory = @"Backup";
        //    _appDataDirectory = @"%localappdata%";
        //    _programFilesDirectory = "%programfiles%";
        //    _relativeAppDataLocation = @"Ubisoft Game Launcher\spool";
        //    _relativeProgramFilesLocation = @"Ubisoft\Ubisoft Game Launcher\savegames";
        //}

    public class BackupCreator
    {
        private readonly string _backupDirectory;
        private readonly string _relativeProgramFilesDirectory;
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

        public BackupCreator(string backupDirectory, string relativeProgramFilesDirectory)
        {
            _backupDirectory = backupDirectory;
            _relativeProgramFilesDirectory = relativeProgramFilesDirectory;
        }

        public BackupCreator(Configuration configuration)
            : this(configuration.BackupDirectory, configuration.RelativeProgramFilesLocation)
        { }

        protected virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }

        protected virtual IBackupDirectoryResolver GetBackupDirectoryResolver()
        {
            return new ProgramFilesDirectoryResolver(_relativeProgramFilesDirectory, _backupDirectory);
        }

        public void CreateBackup(string savegame, string name)
        {
            FileSystem.CreateDirectoryIfNotExists(_backupDirectory);

            var newTimedBackupPath = BackupDirectoryResolver.GetNewTimedBackupPath(name);
            var backupPath = BackupDirectoryResolver.GetBackupPath(savegame, newTimedBackupPath);

            FileSystem.CopyDirectory(savegame, backupPath);
        }
    }

}
