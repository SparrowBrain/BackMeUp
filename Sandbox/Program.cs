using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackMeUp;
using BackMeUp.Utils;

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
            //AppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ProgramFilesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            //RelativeAppDataLocation = @"Ubisoft Game Launcher\spool",
            RelativeProgramFilesLocation = @"Ubisoft\Ubisoft Game Launcher\savegames",
        };

        private static readonly Game[] _games = 
        {
            new Game ("Far Cry 3", 46),
            new Game ("Assasin's Creed IV Back Flag", 437)
        };

        static void Main(string[] args)
        {
            //Backup();
            //Watcher();
            //BackupWatcher();
            //FullBackupJob();

            BackupProcess();
        }

        private static void FullBackupJob()
        {
            Console.WriteLine("{1}{0}-------------------{0}Job started", Environment.NewLine, DateTime.Now);
            var watcher = new SaveWatcher(Configuration);
            var backupWatcher = new BackupWatcher(Configuration);

            var latestSave = watcher.GetLatestSave();
            var saveGameNumber = Path.GetFileName(latestSave);
            var game = _games.FirstOrDefault(x => x.SaveGameNumber.ToString(CultureInfo.InvariantCulture).Equals(saveGameNumber)) ??
                       new Game(Convert.ToInt32(saveGameNumber));

            Console.WriteLine("{0} Game identified {1}", DateTime.Now, game);

            var latestBackupSave = backupWatcher.GetLatestGameSaveBackup(game.Name);

            bool saveBackedUp;
            if (string.IsNullOrEmpty(latestBackupSave))
            {
                saveBackedUp = false;
            }
            else
            {
                var comparer = new Comparer();
                saveBackedUp = comparer.CompareDirectories(latestSave, latestBackupSave);
            }

            if (!saveBackedUp)
            {
                Console.WriteLine("{0} New save found at {1}", DateTime.Now, latestSave);
                var backupCreator = new BackupCreator(Configuration);
                backupCreator.CreateBackup(latestSave, game.Name);
            }
            Console.WriteLine("Done");
            Console.WriteLine();
        }

        private static void BackupProcess()
        {
            var period = new TimeSpan(0, 10, 0);
            while (true)
            {
                FullBackupJob();
                Thread.Sleep(period);
            }
        }
    }
}
