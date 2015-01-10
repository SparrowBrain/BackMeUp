using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Utils;

namespace BackMeUp
{
    public interface IBackupDirectoryResolver
    {
        string GetFullNewBackupDirectory(string saveGameDirecotry, string timedGameDirectory);
        string GetLatestSaveGameBackup(string gameName);
        string GetTimedGameBackupDirectory(string gameName);
    }

    public class BackupDirectoryResolver : IBackupDirectoryResolver
    {
        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string _backupDirectory;

        public BackupDirectoryResolver(string backupDirectory)
        {
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
            if (!Directory.Exists(saveGameDirectory))
            {
                return null;
            }

            var userDirecotry = Directory.GetDirectories(saveGameDirectory).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                return null;
            }

            var latestSaveGame = Directory.GetFileSystemEntries(userDirecotry).FirstOrDefault();

            return latestSaveGame;
        }
        
        public string GetLatestBackup(string gameName)
        {
            gameName = GetDirectoryNameFixer().ReplaceInvalidCharacters(gameName);
            var gameBackupPath = Path.Combine(_backupDirectory, gameName);
            if (!Directory.Exists(gameBackupPath))
                return null;

            var backupDirectories = Directory.GetDirectories(gameBackupPath);

            var validDirecotries = backupDirectories.Where(folder => _backupFolderRegex.IsMatch(Path.GetFileName(folder))).ToList();

            validDirecotries.Sort();
            var latestDirectory = validDirecotries.LastOrDefault();
            return latestDirectory;
        }

        public string GetTimedGameBackupDirectory(string gameName)
        {
            gameName = GetDirectoryNameFixer().ReplaceInvalidCharacters(gameName);
            var now = GetDateTimeWrapper().Now();
            var timedFolderName = string.Format("{0}", now.ToString("yyyy-MM-dd_HHmmss"));
            return Path.Combine(_backupDirectory, gameName, timedFolderName);
        }
    }
}