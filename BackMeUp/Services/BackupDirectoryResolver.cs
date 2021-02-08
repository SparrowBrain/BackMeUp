using System;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex BackupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string _backupDirectory;
        private readonly IDirectory _directory;
        private readonly IDirectoryNameFixer _directoryNameFixer;

        public BackupDirectoryResolver(string backupDirectory, IDirectory directory,
            IDirectoryNameFixer directoryNameFixer)
        {
            _backupDirectory = backupDirectory;
            _directory = directory;
            _directoryNameFixer = directoryNameFixer;
        }

        public string GetSaveFilesBackupPath(string gameSaveFilesDirecotry, string timedGameDirectory)
        {
            if (string.IsNullOrEmpty(gameSaveFilesDirecotry))
            {
                throw new ArgumentException("gameSaveFilesDirectory");
            }
            if (string.IsNullOrEmpty(timedGameDirectory))
            {
                throw new ArgumentException("timedGameDirectory");
            }

            var userDirectory = Directory.GetParent(gameSaveFilesDirecotry).Name;
            var newBackupDirectory = Path.Combine(timedGameDirectory, Constants.SaveGames, userDirectory);

            var saveFilesPath = Path.Combine(newBackupDirectory, Path.GetFileName(gameSaveFilesDirecotry));
            return saveFilesPath;
        }

        public string GetLatestSaveFilesBackupPath(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("gameName");
            }

            var latestBackup = GetLatestBackupPath(gameName);
            if (string.IsNullOrEmpty(latestBackup))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("{0}: No backups found for game {1}", "GetLatestSaveFilesBackupPath()", gameName);
                }
                return null;
            }

            var saveGameDirectory = Path.Combine(latestBackup, Constants.SaveGames);
            if (!_directory.Exists(saveGameDirectory))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("{0}: No savegame directory found {1}", "GetLatestSaveFilesBackupPath()",
                        saveGameDirectory);
                }
                return null;
            }

            var userDirecotry = _directory.GetDirectories(saveGameDirectory).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("{0}: No user directories found {1}", "GetLatestSaveFilesBackupPath()", userDirecotry);
                }
                return null;
            }

            var latestSaveFilesPath = _directory.GetDirectories(userDirecotry).FirstOrDefault();

            return latestSaveFilesPath;
        }

        public string GetNewTimedGameBackupPath(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("gameName");
            }

            gameName = _directoryNameFixer.RemoveInvalidCharacters(gameName);
            var now = SystemTime.Now();
            var timedFolderName = now.ToString(DateTimeFormat);
            return Path.Combine(_backupDirectory, gameName, timedFolderName);
        }

        private string GetLatestBackupPath(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("gameName");
            }

            gameName = _directoryNameFixer.RemoveInvalidCharacters(gameName);
            var gameBackupPath = Path.Combine(_backupDirectory, gameName);
            if (!_directory.Exists(gameBackupPath))
            {
                return null;
            }

            var backupDirectories = _directory.GetDirectories(gameBackupPath);

            var validDirecotries =
                backupDirectories.Where(folder => BackupFolderRegex.IsMatch(Path.GetFileName(folder))).ToList();

            validDirecotries.Sort();
            var latestDirectory = validDirecotries.LastOrDefault();
            return latestDirectory;
        }
    }
}