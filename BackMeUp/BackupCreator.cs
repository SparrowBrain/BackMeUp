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
        private IFileSystem _fileSystem;
        
        protected IFileSystem FileSystem
        {
            get { return _fileSystem ?? (_fileSystem = GetFileSystem()); }
            set { _fileSystem = value; }
        }

        public BackupCreator(string backupDirectory)
        {
            _backupDirectory = backupDirectory;
        }

        public BackupCreator(Configuration configuration)
            : this(configuration.BackupDirectory)
        { }

        protected virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }

        public void CreateBackup(IDictionary<string, IBackupDirectoryResolver> resolverDictionary, string name)
        {
            FileSystem.CreateDirectoryIfNotExists(_backupDirectory);
            var newTimedBackupPath = resolverDictionary.First().Value.GetNewTimedBackupPath(name);

            foreach (var keyValuePair in resolverDictionary)
            {
                var originalPath = keyValuePair.Key;
                var resolver = keyValuePair.Value;
                var backupPath = resolver.GetBackupPath(originalPath, newTimedBackupPath);

                resolver.Copy(originalPath, backupPath);
            }
        }
    }

    public interface IBackupDirectoryResolver
    {
        string GetBackupPath(string originalPath, string backupDirectory);
        string GetLatest(string name);
        string GetNewTimedBackupPath(string name);
        void Copy(string source, string destination);
    }

    public abstract class BackupDirectoryResolver : IBackupDirectoryResolver
    {
        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private string _relativeLocation;
        private string _backupDirectory;
        protected abstract string BackupFolder { get; }

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

        protected virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }

        public string GetBackupPath(string originalPath, string backupDirectory)
        {
            var userDirecotryName = Directory.GetParent(originalPath).Name;
            backupDirectory = Path.Combine(backupDirectory, BackupFolder, _relativeLocation, userDirecotryName);

            var backupPath = Path.Combine(backupDirectory, originalPath);
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

        public string GetNewTimedBackupPath(string name)
        {
            name = GetDirectoryNameFixer().ReplaceInvalidCharacters(name);
            var now = GetDateTimeWrapper().Now();
            var timedFolderName = string.Format("{0}", now.ToString("yyyy-MM-dd_HHmmss"));
            return Path.Combine(_backupDirectory, name, timedFolderName);
        }

        public abstract void Copy(string source, string destination);
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

        public override void Copy(string source, string destination)
        {
            var fileSystem = GetFileSystem();
            fileSystem.CreateDirectoryIfNotExists(Path.GetDirectoryName(source));

            fileSystem.FileCopy(source, destination);
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

        public override void Copy(string source, string destination)
        {
            var fileSystem = GetFileSystem();
            fileSystem.CopyDirectory(source, destination);
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
