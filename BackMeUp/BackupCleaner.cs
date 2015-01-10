using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Utils;

namespace BackMeUp
{
    // TODO: currently this does nothing. Asses whether it is needed.
    public class BackupCleaner
    {
        private readonly string _backupDirectory;

        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public BackupCleaner(Configuration configuration)
        {
            _backupDirectory = configuration.BackupDirectory;
        }

        public string GetLatestBackup(string gameName)
        {
            gameName = new DirectoryNameFixer().ReplaceInvalidCharacters(gameName);
            var gameBackupPath = Path.Combine(_backupDirectory, gameName);
            if (!Directory.Exists(gameBackupPath))
                return null;

            var backupDirectories = Directory.GetDirectories(gameBackupPath);

            var validDirecotries = backupDirectories.Where(folder => _backupFolderRegex.IsMatch(Path.GetFileName(folder))).ToList();

            validDirecotries.Sort((x, y) => String.Compare(x, y, StringComparison.Ordinal));
            var latestDirectory = validDirecotries.LastOrDefault();
            return latestDirectory;
        }
    }
}