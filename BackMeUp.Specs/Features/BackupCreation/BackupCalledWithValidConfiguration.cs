using System;
using System.IO;
using Xunit;

namespace BackMeUp.Specs.Features.BackupCreation
{
    public class BackupCalledWithValidConfiguration : IDisposable
    {
        private readonly string _backupDirectory;
        private readonly string _appDataDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeAppDataLocation;
        private readonly string _relativeProgramFilesLocation;
        public BackupCalledWithValidConfiguration()
        {
            _backupDirectory = @"Backup";
            _appDataDirectory = @"%appdata%\..\Local";
            _programFilesDirectory = "%programfiles%";
            _relativeAppDataLocation = @"Ubisoft Game Launcher\spool";
            _relativeProgramFilesLocation = @"Ubisoft\Ubisoft Game Launcher\savegames";
        }

        [Fact(DisplayName = "Backup directory created if it doesn't exist")]
        public void BackupDirectoryCreatedIfNotExists()
        {
            var backup = new BackupCreator(_backupDirectory, _appDataDirectory, _programFilesDirectory,
                _relativeAppDataLocation, _relativeProgramFilesLocation);
            backup.CreateBackup();

            Assert.True(new DirectoryInfo(_backupDirectory).Exists);
        }

        [Fact(DisplayName = "No exception if Backup directory already exists")]
        public void BackupDirectoryCreatedIfNotExists()
        {
            var backup = new BackupCreator(_backupDirectory, _appDataDirectory, _programFilesDirectory,
                _relativeAppDataLocation, _relativeProgramFilesLocation);
            backup.CreateBackup();

            Assert.True(new DirectoryInfo(_backupDirectory).Exists);
        }

        [Fact(DisplayName = "Backup location created")]
        public void BackupLocationCreated()
        {
            throw new NotImplementedException("Must implement test");
        }

        [Fact(DisplayName = "AppData files copied to backup location")]
        public void AppDataFilesAreCopiedToBackupLocation()
        {
            throw new NotImplementedException("Must implement test");
        }

        [Fact(DisplayName = "ProgramFiles files copied to backup location")]
        public void ProgramFilesFilesAreCopiedToBackupLocation()
        {
            throw new NotImplementedException("Must implement test");
        }

        public void Dispose()
        {
            var backupDirectory = new DirectoryInfo(_backupDirectory);
            if (backupDirectory.Exists) backupDirectory.Delete((true));

        }
    }

    public class Configuration
    {
        public string BackupDirectory { get; set; }
        public string AppDataDirectory { get; set; }
        public string ProgramFilesDirectory { get; set; }
    }

    public class BackupCreator
    {
        private readonly string _backupDirectory;
        private readonly string _appDataDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeAppDataLocation;
        private readonly string _relativeProgramFilesLocation;

        public BackupCreator(string backupDirectory, string appDataDirectory, string programFilesDirectory, string relativeAppDataLocation, string relativeProgramFilesLocation)
        {
            _backupDirectory = backupDirectory;
            _appDataDirectory = appDataDirectory;
            _programFilesDirectory = programFilesDirectory;
            _relativeAppDataLocation = relativeAppDataLocation;
            _relativeProgramFilesLocation = relativeProgramFilesLocation;
        }

        public void CreateBackup()
        {
            var directoryInfo = new DirectoryInfo(_backupDirectory);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }
    }

    public class FileHandler
    {
        public FileHandler()
        {
                
        }
    }

    public class backupCreator
    {
        [Fact(DisplayName = "")]
        public void CreateBackup()
        {
            throw new NotImplementedException("Must implement test");
        }
    }
}
