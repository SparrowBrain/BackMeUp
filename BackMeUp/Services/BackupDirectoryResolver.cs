using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Data;
using BackMeUp.Utils;
using BackMeUp.Wrappers;

namespace BackMeUp.Services
{
    public interface IBackupDirectoryResolver
    {
        string GetFullNewBackupDirectory(string saveGameDirecotry, string timedGameDirectory);
        string GetLatestSaveGameBackup(string gameName);
        string GetTimedGameBackupDirectory(string gameName);
    }

    public class BackupDirectoryResolver : IBackupDirectoryResolver
    {
        private const string DateTimeFormat = "yyyy-MM-dd_HHmmss";
        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string _backupDirectory;
        private readonly IFileSystem _fileSystem;
        private readonly IDirectoryNameFixer _directoryNameFixer;

        public BackupDirectoryResolver(string backupDirectory, IFileSystem fileSystem, IDirectoryNameFixer directoryNameFixer)
        {
            _backupDirectory = backupDirectory;
            _fileSystem = fileSystem;
            _directoryNameFixer = directoryNameFixer;
        }

        public string GetFullNewBackupDirectory(string saveGameDirecotry, string timedGameDirectory)
        {
            var userDirectory = Directory.GetParent(saveGameDirecotry).Name;
            var newBackupDirectory = Path.Combine(timedGameDirectory, Constants.SaveGames, userDirectory);

            var backupPath = Path.Combine(newBackupDirectory, Path.GetFileName(saveGameDirecotry));
            return backupPath;
        }

        public string GetLatestSaveGameBackup(string gameName)
        {
            var latestBackup = GetLatestBackup(gameName);
            if (string.IsNullOrEmpty(latestBackup))
            {
                return null;
            }

            var saveGameDirectory = Path.Combine(latestBackup, Constants.SaveGames);
            if (!_fileSystem.DirectoryExists(saveGameDirectory))
            {
                return null;
            }

            var userDirecotry = _fileSystem.DirectoryGetDirectories(saveGameDirectory).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                return null;
            }

            var latestSaveGame = _fileSystem.DirectoryGetFileSystemEntries(userDirecotry).FirstOrDefault();

            return latestSaveGame;
        }
        
        public string GetLatestBackup(string gameName)
        {
            gameName = _directoryNameFixer.ReplaceInvalidCharacters(gameName);
            var gameBackupPath = Path.Combine(_backupDirectory, gameName);
            if (!_fileSystem.DirectoryExists(gameBackupPath))
                return null;

            var backupDirectories = _fileSystem.DirectoryGetDirectories(gameBackupPath);

            var validDirecotries = backupDirectories.Where(folder => _backupFolderRegex.IsMatch(Path.GetFileName(folder))).ToList();

            validDirecotries.Sort();
            var latestDirectory = validDirecotries.LastOrDefault();
            return latestDirectory;
        }

        public string GetTimedGameBackupDirectory(string gameName)
        {
            gameName = _directoryNameFixer.ReplaceInvalidCharacters(gameName);
            var now = SystemTime.Now();
            var timedFolderName = string.Format("{0}", now.ToString(DateTimeFormat));
            return Path.Combine(_backupDirectory, gameName, timedFolderName);
        }
    }
}