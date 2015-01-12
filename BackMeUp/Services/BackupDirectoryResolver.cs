using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Data;
using BackMeUp.Utils;
using BackMeUp.Wrappers;
using NLog;

namespace BackMeUp.Services
{
    public class BackupDirectoryResolver : IBackupDirectoryResolver
    {
        private const string DateTimeFormat = "yyyy-MM-dd_HHmmss";

        private readonly string _backupDirectory;

        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly IDirectory _directory;
        private readonly IDirectoryNameFixer _directoryNameFixer;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BackupDirectoryResolver(string backupDirectory, IDirectory directory,
            IDirectoryNameFixer directoryNameFixer)
        {
            _backupDirectory = backupDirectory;
            _directory = directory;
            _directoryNameFixer = directoryNameFixer;
        }

        public string GetSaveFilesBackupPath(string saveGameDirecotry, string timedGameDirectory)
        {
            var userDirectory = Directory.GetParent(saveGameDirecotry).Name;
            var newBackupDirectory = Path.Combine(timedGameDirectory, Constants.SaveGames, userDirectory);

            var saveFilesPath = Path.Combine(newBackupDirectory, Path.GetFileName(saveGameDirecotry));
            return saveFilesPath;
        }

        public string GetLatestSaveFilesBackupPath(string gameName)
        {
            var latestBackup = GetLatestBackupPath(gameName);
            if (string.IsNullOrEmpty(latestBackup))
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("{0}: No backups found for game {1}", "GetLatestSaveFilesBackupPath()", gameName);
                }
                return null;
            }

            var saveGameDirectory = Path.Combine(latestBackup, Constants.SaveGames);
            if (!_directory.Exists(saveGameDirectory))
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("{0}: No savegame directory found {1}", "GetLatestSaveFilesBackupPath()",
                        saveGameDirectory);
                }
                return null;
            }

            var userDirecotry = _directory.GetDirectories(saveGameDirectory).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("{0}: No user directories found {1}", "GetLatestSaveFilesBackupPath()", userDirecotry);
                }
                return null;
            }

            var latestSaveFilesPath = _directory.GetFileSystemEntries(userDirecotry).FirstOrDefault();

            return latestSaveFilesPath;
        }

        public string GetNewTimedGameBackupPath(string gameName)
        {
            gameName = _directoryNameFixer.ReplaceInvalidCharacters(gameName);
            var now = SystemTime.Now();
            var timedFolderName = string.Format("{0}", now.ToString(DateTimeFormat));
            return Path.Combine(_backupDirectory, gameName, timedFolderName);
        }

        public string GetLatestBackupPath(string gameName)
        {
            gameName = _directoryNameFixer.ReplaceInvalidCharacters(gameName);
            var gameBackupPath = Path.Combine(_backupDirectory, gameName);
            if (!_directory.Exists(gameBackupPath))
            {
                return null;
            }

            var backupDirectories = _directory.GetDirectories(gameBackupPath);

            var validDirecotries =
                backupDirectories.Where(folder => _backupFolderRegex.IsMatch(Path.GetFileName(folder))).ToList();

            validDirecotries.Sort();
            var latestDirectory = validDirecotries.LastOrDefault();
            return latestDirectory;
        }
    }
}