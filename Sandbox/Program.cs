using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BackMeUp;

namespace Sandbox
{
    class Program
    {
        private const string BackupDirectory = @"Backup";
        private const string RelativeAppDataLocation = @"Ubisoft Game Launcher\spool";
        private const string RelativeProgramFilesLocation = @"Ubisoft\Ubisoft Game Launcher\savegames";
        private const string MyUserDir = "45922324-ba0e-489b-b7a9-1a09511c7b45";

        private static readonly Configuration Configuration = new Configuration
        {
            BackupDirectory = "Backup",
            AppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ProgramFilesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            RelativeAppDataLocation = @"Ubisoft Game Launcher\spool",
            RelativeProgramFilesLocation = @"Ubisoft\Ubisoft Game Launcher\savegames",
        };

        private static readonly Game[] _games = 
        {
            new Game {Name = "Far Cry 3", SpoolNumber = 101, GameSaveNumber = 46},
            new Game {Name = "Assasin's Creed IV Back Flag", SpoolNumber = 620, GameSaveNumber = 437}
        };

        static void Main(string[] args)
        {
            //Backup();
            //Watcher();
            BackupWatcher();
        }

        private static void Backup()
        {
            var backupCreator = new BackupCreator(BackupDirectory, RelativeAppDataLocation, RelativeProgramFilesLocation);

            var spoolFileInfo = new FileInfo(GetGamePaths(_games[0].Name).Item1);
            var savegameDirecotryInfo = new DirectoryInfo(GetGamePaths(_games[0].Name).Item2);
            backupCreator.CreateBackup(spoolFileInfo, savegameDirecotryInfo, _games[0].Name);

            spoolFileInfo = new FileInfo(GetGamePaths(_games[1].Name).Item1);
            savegameDirecotryInfo = new DirectoryInfo(GetGamePaths(_games[1].Name).Item2);
            backupCreator.CreateBackup(spoolFileInfo, savegameDirecotryInfo, _games[1].Name);
        }

        private static Tuple<string, string> GetGamePaths(string name)
        {
            var game = _games.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (game == null) throw new Exception("I give up!");

            var spool = Path.Combine(Configuration.AppDataDirectory, RelativeAppDataLocation, MyUserDir, string.Format("{0}.spool", game.SpoolNumber));
            var saveGame = Path.Combine(Configuration.ProgramFilesDirectory, RelativeProgramFilesLocation, MyUserDir, game.GameSaveNumber.ToString(CultureInfo.InvariantCulture));

            return Tuple.Create(spool, saveGame);
        }

        private static void Watcher()
        {
            var watcher = new Watcher(Configuration);
            Console.WriteLine(watcher.GetLatestSave());
            Console.WriteLine(watcher.GetLatestSpool());
            Console.ReadKey();
        }

        private static void BackupWatcher()
        {
            var backupWatcher = new BackupWatcher(Configuration);
            Console.WriteLine(backupWatcher.GetLatestGameSaveBackup("Far Cry 3"));
            Console.WriteLine(backupWatcher.GetLatestSpoolBackup("Far Cry 3"));

            Console.WriteLine(backupWatcher.GetLatestGameSaveBackup(_games[1].Name));
            Console.WriteLine(backupWatcher.GetLatestSpoolBackup(_games[1].Name));

            Console.ReadKey();
        }
    }
}
