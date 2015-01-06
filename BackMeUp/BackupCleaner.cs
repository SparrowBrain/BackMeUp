using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BackMeUp.Utils;

namespace BackMeUp
{
    public class BackupCleaner
    {
        private readonly string _backupDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeProgramFilesLocation;

        private readonly Regex _backupFolderRegex = new Regex(@"\d{4}-\d{2}-\d{2}_\d{6}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public BackupCleaner(Configuration configuration)
        {
            _backupDirectory = configuration.BackupDirectory;
            _programFilesDirectory = configuration.ProgramFilesDirectory;
            _relativeProgramFilesLocation = configuration.RelativeProgramFilesLocation;
        }

        public string GetLatestBackup(string name)
        {
            name = new DirectoryNameFixer().ReplaceInvalidCharacters(name);
            var gameBackupPath = Path.Combine(_backupDirectory, name);
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