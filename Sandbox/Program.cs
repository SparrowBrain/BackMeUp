using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackMeUp;
using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using BackMeUp.Wrappers;

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
            BackupDirectory = "H:\\Backup",
            SaveGameDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Ubisoft\Ubisoft Game Launcher", Constants.SaveGames)
        };

        private static readonly Game[] _games = 
        {
            new Game ("Far Cry 3", 46),
            new Game ("Assasin's Creed IV Back Flag", 437)
        };

        private static readonly IFileSystem FileSystem = new FileSystem();
        private static readonly IBackupDirectoryResolver BackupDirectoryResolver = new BackupDirectoryResolver(Configuration.BackupDirectory, FileSystem, new DirectoryNameFixer(),new DateTimeWrapper());
        

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
            var saveWatcher = new SaveWatcher(Configuration, FileSystem);
            var backupWatcher = new BackupWatcher(BackupDirectoryResolver);

            var latestSave = saveWatcher.GetLatestSaveFilesDirecotry();
            if (string.IsNullOrEmpty(latestSave))
            {
                Console.WriteLine("No saves found.");
                return;
            }

            var saveGameNumber = Path.GetFileName(latestSave);
            var game = _games.FirstOrDefault(x => x.SaveGameNumber.ToString(CultureInfo.InvariantCulture).Equals(saveGameNumber)) ??
                       new Game(Convert.ToInt32(saveGameNumber));

            Console.WriteLine("{0} Game identified {1} for last save", DateTime.Now, game);

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
                var backupCreator = new BackupCreator(Configuration, BackupDirectoryResolver, FileSystem);
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
