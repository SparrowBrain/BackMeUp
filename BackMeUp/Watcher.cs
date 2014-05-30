using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackMeUp
{
    public class Watcher
    {
        private readonly string _backupDirectory;
        private readonly string _appDataDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeAppDataLocation;
        private readonly string _relativeProgramFilesLocation;
        
        public Watcher(string backupDirectory, string appDataDirectory, string programFilesDirectory, string relativeAppDataLocation, string relativeProgramFilesLocation)
        {
            _backupDirectory = backupDirectory;
            _appDataDirectory = appDataDirectory;
            _programFilesDirectory = programFilesDirectory;
            _relativeAppDataLocation = relativeAppDataLocation;
            _relativeProgramFilesLocation = relativeProgramFilesLocation;
        }

        public Watcher(Configuration configuration)
            : this(
                configuration.BackupDirectory, configuration.AppDataDirectory, configuration.ProgramFilesDirectory,
                configuration.RelativeAppDataLocation, configuration.RelativeProgramFilesLocation)
        {
        }

        public string GetLatestSave()
        {
            var saveGamesPath = Path.Combine(_programFilesDirectory, _relativeProgramFilesLocation);
            var saveGamesDirecotries =
                Directory.GetDirectories(saveGamesPath, "*", SearchOption.AllDirectories)
                    .Where(x => Path.GetFileName(x).All(char.IsDigit));

            var lastWriteTime=DateTime.MinValue;
            string latestSave = null;
            foreach (var gameSave in saveGamesDirecotries)
            {
                var directoryinfo = new DirectoryInfo(gameSave);
                if (directoryinfo.LastWriteTime > lastWriteTime)
                {
                    lastWriteTime = directoryinfo.LastWriteTime;
                    latestSave = gameSave;
                }
            }

            return latestSave;
        }

        public string GetLatestSpool()
        {
            var spoolPath = Path.Combine(_appDataDirectory, _relativeAppDataLocation);
            var spoolFiles = Directory.GetFiles(spoolPath, "*.spool", SearchOption.AllDirectories);

            var lastWriteTime = DateTime.MinValue;
            string latestFile = null;
            foreach (var spoolFile in spoolFiles)
            {
                var fileInfo = new FileInfo(spoolFile);
                if (fileInfo.LastWriteTime > lastWriteTime)
                {
                    lastWriteTime = fileInfo.LastWriteTime;
                    latestFile = spoolFile;
                }
            }

            return latestFile;
        }
    }


}
