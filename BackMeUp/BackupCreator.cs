using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        
        protected IFileSystem FileSystem
        {
            get { return _fileSystem ?? (_fileSystem = GetFileSystem()); }
            set { _fileSystem = value; }
        }

        public BackupCreator(string backupDirectory, string relativeAppDataLocation, string relativeProgramFilesLocation, IList<IBackupDirectoryResolver> direcotryResolvers)
        {
            _backupDirectory = backupDirectory;
            _relativeAppDataLocation = relativeAppDataLocation;
            _relativeProgramFilesLocation = relativeProgramFilesLocation;
        }

        public BackupCreator(Configuration configuration)
            : this(configuration.BackupDirectory, configuration.RelativeAppDataLocation, configuration.RelativeProgramFilesLocation)
        { }

        protected virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }

        public void CreateBackup(IList<IBackupDirectoryResolver> _backupDirectoryResolvers, string name)
        {
            FileSystem.CreateDirectoryIfNotExists(_backupDirectory);

            var backupDirectory = _backupDirectoryResolvers.First().GetNewBackupName(name);

            foreach (var resolver in _backupDirectoryResolvers)
            {
                var backupPath = resolver.GetBackupPath(backupDirectory);
                //FileSystem.CreateDirectoryIfNotExists(appDataBackupFileInfo.DirectoryName);
                FileSystem.FileCopy(spoolFileInfo.FullName, backupPath);
            }

            var appDataBackupFileInfo = GetAppDataBackupPath(spoolFileInfo, backupDirectory);
            var programFilsBackupPath = GetProgramFilesBackupPath(savegamesDirectoryInfo, backupDirectory);

            FileSystem.CreateDirectoryIfNotExists(appDataBackupFileInfo.DirectoryName);

            FileSystem.FileCopy(spoolFileInfo.FullName, appDataBackupFileInfo.FullName);
            FileSystem.CopyDirectory(savegamesDirectoryInfo.FullName, programFilsBackupPath);
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

    public interface IBackupDirectoryResolver
    {
        string GetBackupPath(string pathToBackup, string backupDirectory);
        string GetLatest(string name);
        string GetNewBackupName(string name);
    }

    public abstract class BackupDirectoryResolver : IBackupDirectoryResolver
    {
        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private string _relativeLocation;
        private string _backupDirectory;
        protected virtual string BackupFolder { get{return null;}  }

        public BackupDirectoryResolver(string relativeLocation, string backupDirectory)
        {
            _relativeLocation = relativeLocation;
            _backupDirectory = backupDirectory;
        }

        protected virtual IDirectoryNameFixer GetDirectoryNameFixer()
        {
            return new DirectoryNameFixer();
        }

        protected virtual IDateTime GetDateTimeWrapper()
        {
            return new DateTimeWrapper();
        }

        public string GetBackupPath(string pathToBackup, string backupDirectory)
        {
            var userDirecotryName = Directory.GetParent(pathToBackup).Name;
            backupDirectory = Path.Combine(backupDirectory, BackupFolder, _relativeLocation, userDirecotryName);

            var backupPath = Path.Combine(backupDirectory, pathToBackup);
            return backupPath;
        }

        public string GetLatest(string name)
        {
            var latestBackup = GetLatestBackup(name);
            if (string.IsNullOrEmpty(latestBackup))
            {
                return null;
            }

            var relativeBackupPath = Path.Combine(latestBackup, BackupFolder, _relativeLocation);
            if (!Directory.Exists(relativeBackupPath))
            {
                return null;
            }

            var userDirecotry = Directory.GetDirectories(relativeBackupPath).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                return null;
            }

            var latest = Directory.GetFileSystemEntries(userDirecotry).FirstOrDefault();

            return latest;
        }
        
        public string GetLatestBackup(string name)
        {
            name = GetDirectoryNameFixer().ReplaceInvalidCharacters(name);
            var gameBackupPath = Path.Combine(_backupDirectory, name);
            if (!Directory.Exists(gameBackupPath))
                return null;

            var backupDirectories = Directory.GetDirectories(gameBackupPath);

            var validDirecotries = backupDirectories.Where(folder => _backupFolderRegex.IsMatch(Path.GetFileName(folder))).ToList();

            validDirecotries.Sort();
            var latestDirectory = validDirecotries.LastOrDefault();
            return latestDirectory;
        }

        public string GetNewBackupName(string name)
        {
            name = GetDirectoryNameFixer().ReplaceInvalidCharacters(name);
            var now = GetDateTimeWrapper().Now();
            var backupFolderName = string.Format("{0}", now.ToString("yyyy-MM-dd_HHmmss"));
            return Path.Combine(_backupDirectory, name, backupFolderName);
        }
    }

    public class AppDataDirectoryResolver : BackupDirectoryResolver
    {
        public AppDataDirectoryResolver(string relativeLocation,  string backupDirectory) : base(relativeLocation, backupDirectory)
        {
        }

        protected override string BackupFolder
        {
            get
            {
                return "AppData";
            }
        }
    }

    public class ProgramFilesDirectoryResolver : BackupDirectoryResolver
    {
        public ProgramFilesDirectoryResolver(string relativeLocation, string backupDirectory)
            : base(relativeLocation, backupDirectory)
        {
        }

        protected override string BackupFolder
        {
            get
            {
                return "ProgramFiles";
            }
        }
    }

    public interface IDateTime
    {
        DateTime Now();
    }

    public class DateTimeWrapper:IDateTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
