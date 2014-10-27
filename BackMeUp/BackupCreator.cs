using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BackMeUp.Utils;

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
        private readonly string _relativeAppDataLocation;
        private readonly string _relativeProgramFilesLocation;

        private IFileSystem _fileSystem;
        private IDirectoryNameFixer _directoryNameFixer;

        public BackupCreator(string backupDirectory, string relativeAppDataLocation, string relativeProgramFilesLocation)
        {
            _backupDirectory = backupDirectory;
            _relativeAppDataLocation = relativeAppDataLocation;
            _relativeProgramFilesLocation = relativeProgramFilesLocation;

            _fileSystem = GetFileSystem();
            _directoryNameFixer = GetDirectoryNameFixer();
        }

        public BackupCreator(Configuration configuration):this(configuration.BackupDirectory, configuration.RelativeAppDataLocation,configuration.RelativeProgramFilesLocation)
        { }

        public virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }

        public virtual IDirectoryNameFixer GetDirectoryNameFixer()
        {
            return new DirectoryNameFixer();
        }

        public void CreateBackup(FileInfo spoolFileInfo, DirectoryInfo savegamesDirectoryInfo, string name)
        {
            _fileSystem.CreateDirectoryIfNotExists(_backupDirectory);

            name = _directoryNameFixer.ReplaceInvalidCharacters(name);
            var backupFolderName = string.Format("{0}", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));/////////////DateTime, lol ^-^
            var backupDirectory = Path.Combine(_backupDirectory, name, backupFolderName);

            var appDataBackupFileInfo = GetAppDataBackupPath(spoolFileInfo, backupDirectory);
            var programFilsBackupPath = GetProgramFilesBackupPath(savegamesDirectoryInfo, backupDirectory);

            _fileSystem.CreateDirectoryIfNotExists(appDataBackupFileInfo.DirectoryName);

            _fileSystem.FileCopy(spoolFileInfo.FullName, appDataBackupFileInfo.FullName);
            CopyDirectory(savegamesDirectoryInfo.FullName, programFilsBackupPath);
        }

        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            _fileSystem.CreateDirectoryIfNotExists(destinationDirectory);

            foreach (var file in _fileSystem.DirectoryGetFiles(sourceDirectory))
            {
                var fileName = Path.GetFileName(file);
                _fileSystem.FileCopy(file, Path.Combine(destinationDirectory, fileName));
            }
        }

        public FileInfo GetAppDataBackupPath(FileInfo spoolFileInfo, string backupDirectory)
        {
            var userDirecotryName = spoolFileInfo.Directory.Name;
            var spoolBackupDirectory = Path.Combine(backupDirectory, "AppData", _relativeAppDataLocation, userDirecotryName);

            var backupPath = Path.Combine(spoolBackupDirectory, spoolFileInfo.Name);
            return new FileInfo(backupPath);
        }

        public string GetProgramFilesBackupPath(DirectoryInfo savegameDirectoryInfo, string backupDirectory)
        {
            var userDirecotryName = savegameDirectoryInfo.Parent.Name;
            var savegameBackupDirectory = Path.Combine(backupDirectory, "ProgramFiles", _relativeProgramFilesLocation, userDirecotryName);

            var backupPath = Path.Combine(savegameBackupDirectory, savegameDirectoryInfo.Name);
            return backupPath;
        }
    }
}
