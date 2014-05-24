using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackMeUp
{
        //    public BackupCalledWithValidConfiguration()
        //{
        //    _backupDirectory = @"Backup";
        //    _appDataDirectory = @"%localappdata%";
        //    _programFilesDirectory = "%programfiles%";
        //    _relativeAppDataLocation = @"Ubisoft Game Launcher\spool";
        //    _relativeProgramFilesLocation = @"Ubisoft\Ubisoft Game Launcher\savegames";
        //}

    public class BackupCreator
    {
        private readonly string _backupDirectory;
        private readonly string _appDataDirectory;
        private readonly string _programFilesDirectory;
        private readonly string _relativeAppDataLocation;
        private readonly string _relativeProgramFilesLocation;

        public BackupCreator(string backupDirectory, string appDataDirectory, string programFilesDirectory, string relativeAppDataLocation, string relativeProgramFilesLocation)
        {
            _backupDirectory = backupDirectory;
            _appDataDirectory = appDataDirectory;
            _programFilesDirectory = programFilesDirectory;
            _relativeAppDataLocation = relativeAppDataLocation;
            _relativeProgramFilesLocation = relativeProgramFilesLocation;
        }

        public void CreateBackup(string spoolFileName, string savegamesDirectoryName, string name)
        {
            var directoryInfo = new DirectoryInfo(_backupDirectory);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            var backupFolderName = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"), name);

            var appDataPaths = GetAppDataPaths(spoolFileName, Path.Combine(_backupDirectory, backupFolderName));
            
            var programFilesSource = Path.Combine(_programFilesDirectory, _relativeProgramFilesLocation, savegamesDirectoryName);
            var programFilesDestination = Path.Combine(_backupDirectory, backupFolderName, _relativeProgramFilesLocation, savegamesDirectoryName);

            if (!Directory.Exists(Path.GetDirectoryName(appDataPaths.Item2)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(appDataPaths.Item2));
            }
            File.Copy(appDataPaths.Item1, appDataPaths.Item2);
            //CopyDirectory(programFilesSource, programFilesDestination);
        }

        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                var fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(destinationDirectory, fileName));
            }
        }

        public Tuple<string, string> GetAppDataPaths(string spoolFileName, string backupDirectory)
        {
            var appData = new DirectoryInfo(AppDataSourceDirectory);
            var fileInfos = appData.GetFileSystemInfos(spoolFileName, SearchOption.AllDirectories);

            var spoolFileInfo = fileInfos.OrderByDescending(x => x.LastWriteTime).FirstOrDefault();
            var scrambledDirectoryName = new DirectoryInfo(Path.GetDirectoryName(spoolFileInfo.FullName)).Name;

            var source = Path.Combine(AppDataSourceDirectory, scrambledDirectoryName, spoolFileName);
            var destination = Path.Combine(backupDirectory, _relativeAppDataLocation, scrambledDirectoryName, spoolFileName);

            return Tuple.Create(source, destination);
        }

        public string AppDataSourceDirectory
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _relativeAppDataLocation); }
        }

        public string ProgramFilesSourceDirectory
        {
            get { return Path.Combine(_programFilesDirectory, _relativeProgramFilesLocation); }
        }
    }


    public class Configuration
    {
        public string BackupDirectory { get; set; }
        public string AppDataDirectory { get; set; }
        public string ProgramFilesDirectory { get; set; }
        public string RelativeAppDataLocation { get; set; }
        public string RelativeProgramFilesLocation { get; set; }
    }
}
