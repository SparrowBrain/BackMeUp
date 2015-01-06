using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Utils;

namespace BackMeUp
{
    public interface IBackupDirectoryResolver
    {
        string GetBackupPath(string originalPath, string backupDirectory);
        string GetLatest(string name);
        string GetNewTimedBackupPath(string name);
    }

    public abstract class BackupDirectoryResolver : IBackupDirectoryResolver
    {
        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string _relativeLocation;
        private readonly string _backupDirectory;
        protected abstract string BackupFolder { get; }

        protected BackupDirectoryResolver(string relativeLocation, string backupDirectory)
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
    }
}