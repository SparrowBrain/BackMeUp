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

    }
}