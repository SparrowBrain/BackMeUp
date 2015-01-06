using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackMeUp
{
    public class SaveWatcher
    {
        private readonly string _backupDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeProgramFilesLocation;
        
        public SaveWatcher(string backupDirectory, string programFilesDirectory, string relativeProgramFilesLocation)
        {
            _backupDirectory = backupDirectory;
            _programFilesDirectory = programFilesDirectory;
            _relativeProgramFilesLocation = relativeProgramFilesLocation;
        }

        public SaveWatcher(Configuration configuration)
            : this(
                configuration.BackupDirectory, configuration.ProgramFilesDirectory, configuration.RelativeProgramFilesLocation)
        {
        }

        // TODO: what happens if there are no saves?
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
    }
}
