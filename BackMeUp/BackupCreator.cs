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

        public BackupCreator(string backupDirectory, string relativeAppDataLocation, string relativeProgramFilesLocation)
        {
            _backupDirectory = backupDirectory;
            _relativeAppDataLocation = relativeAppDataLocation;
            _relativeProgramFilesLocation = relativeProgramFilesLocation;
        }

        public BackupCreator(Configuration configuration):this(configuration.BackupDirectory, configuration.RelativeAppDataLocation,configuration.RelativeProgramFilesLocation)
        { }

        public void CreateBackup(FileInfo spoolFileInfo, DirectoryInfo savegamesDirectoryInfo, string name)
        {
            var directoryInfo = new DirectoryInfo(_backupDirectory);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            name = new DirectoryNameFixer().ReplaceInvalidCharacters(name);
            var backupFolderName = string.Format("{0}", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
            var backupDirectory = Path.Combine(_backupDirectory, name, backupFolderName);

            var appDataBackupFileInfo = GetAppDataBackupPath(spoolFileInfo, backupDirectory);
            var programFilsBackupPath = GetProgramFilesBackupPath(savegamesDirectoryInfo, backupDirectory);

            if (!Directory.Exists(appDataBackupFileInfo.DirectoryName))
            {
                Directory.CreateDirectory(appDataBackupFileInfo.DirectoryName);
            }

            File.Copy(spoolFileInfo.FullName, appDataBackupFileInfo.FullName);
            CopyDirectory(savegamesDirectoryInfo.FullName, programFilsBackupPath);
        }

        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                var fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(destinationDirectory, fileName));
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
