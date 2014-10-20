using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Utils;

namespace BackMeUp
{
    public class BackupWatcher
    {
        private readonly string _backupDirectory;
        private readonly string _appDataDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeAppDataLocation;
        private readonly string _relativeProgramFilesLocation;

        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public BackupWatcher(Configuration configuration)
        {
            _backupDirectory = configuration.BackupDirectory;
            _appDataDirectory = configuration.AppDataDirectory;
            _programFilesDirectory = configuration.ProgramFilesDirectory;
            _relativeAppDataLocation = configuration.RelativeAppDataLocation;
            _relativeProgramFilesLocation = configuration.RelativeProgramFilesLocation;
        }

        public string GetLatestGameSaveBackup(string name)
        {
            var gameBackupPath = GetLatestBackup(name);
            if (string.IsNullOrEmpty(gameBackupPath))
            {
                return null;
            }

            var saveGameDirecotry = Path.Combine(gameBackupPath, "ProgramFiles", _relativeProgramFilesLocation);
            if (!Directory.Exists(saveGameDirecotry))
            {
                return null;
            }

            var userDirecotry = Directory.GetDirectories(saveGameDirecotry).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                return null;
            }

            var latestSaveGame = Directory.GetDirectories(userDirecotry).FirstOrDefault();

            return latestSaveGame;
        }

        public string GetLatestSpoolBackup(string name)
        {
            var gameBackupPath = GetLatestBackup(name);
            if (string.IsNullOrEmpty(gameBackupPath))
            {
                return null;
            }

            var spoolDirectory = Path.Combine(gameBackupPath, "AppData", _relativeAppDataLocation);
            if (!Directory.Exists(spoolDirectory))
            {
                return null;
            }

            var userDirecotry = Directory.GetDirectories(spoolDirectory).FirstOrDefault();
            if (string.IsNullOrEmpty(userDirecotry))
            {
                return null;
            }

            var latestSpool = Directory.GetFiles(userDirecotry, "*.spool").FirstOrDefault();
            return latestSpool;
        }

        public string GetLatestBackup(string name)
        {
            name = new DirectoryNameFixer().ReplaceInvalidCharacters(name);
            var gameBackupPath = Path.Combine(_backupDirectory, name);
            if (!Directory.Exists(gameBackupPath))
                return null;

            var backupDirectories = Directory.GetDirectories(gameBackupPath);

            var validDirecotries = backupDirectories.Where(folder => _backupFolderRegex.IsMatch(Path.GetFileName(folder))).ToList();

            validDirecotries.Sort();
            var latestDirectory = validDirecotries.LastOrDefault();
            return latestDirectory;
        }
    }
}