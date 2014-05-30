using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackMeUp;

namespace Sandbox
{
    class Program
    {
        static string _backupDirectory = @"Backup";
        static string _appDataDirectory = @"%localappdata%";
        static string _programFilesDirectory = "%programfiles%";
        static string _relativeAppDataLocation = @"Ubisoft Game Launcher\spool";
        static string _relativeProgramFilesLocation = @"Ubisoft\Ubisoft Game Launcher\savegames";
        private static string _myUserDir = "45922324-ba0e-489b-b7a9-1a09511c7b45";

        static void Main(string[] args)
        {
            Backup();
        }

        private static void Backup()
        {
            var backupCreator = new BackupCreator(_backupDirectory, _appDataDirectory, _programFilesDirectory, _relativeAppDataLocation, _relativeProgramFilesLocation);

            var spoolFileInfo = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _relativeAppDataLocation, _myUserDir, "101.spool"));
            var savegameDirecotryInfo = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), _relativeProgramFilesLocation, _myUserDir, "46"));
            backupCreator.CreateBackup(spoolFileInfo, savegameDirecotryInfo, "Far Cry 3");
        }
    }
}
