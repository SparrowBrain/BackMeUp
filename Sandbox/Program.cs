using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using BackMeUp.Wrappers;
using Newtonsoft.Json;

namespace Sandbox
{
    class Program
    {
        private static readonly MainConfiguration Configuration = new MainConfiguration(
            "E:\\Backup",
            TimeSpan.FromMinutes(10));
        
        private static readonly IFile SystemFile = new SystemFile();
        private static readonly IDirectory SystemDirectory = new SystemDirectory();
        private static readonly IBackupDirectoryResolver BackupDirectoryResolver = new BackupDirectoryResolver(Configuration.BackupDirectory, SystemDirectory, new DirectoryNameFixer());
        private static readonly IFileOperationHelper FileOperationsHelper = new FileOperationHelper(SystemFile, SystemDirectory);
        

        static void Main(string[] args)
        {
            BackupProcess();
        }

        private static List<Game> ReadGamesList()
        {
            
            try
            {
                var games = JsonConvert.DeserializeObject<GamesConfiguration>(File.ReadAllText("games.json"));
                return games.Games;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured while reading games configuration.");
                Console.WriteLine(e);
                Console.ReadKey();
                throw;
            }
        }

        private static void FullBackupJob(List<Game> games)
        {
            Console.WriteLine("{1}{0}-------------------{0}Job started", Environment.NewLine, DateTime.Now);

            var saveGamesDirectory = Path.Combine(new UPlayPathResolver().GetUPlayInstallationDirectory(), Constants.SaveGames);
            var saveWatcher = new SaveWatcher(saveGamesDirectory, SystemDirectory);
            var backupWatcher = new BackupWatcher(BackupDirectoryResolver);

            var latestSave = saveWatcher.GetLatestSaveFilesPath();
            if (string.IsNullOrEmpty(latestSave))
            {
                Console.WriteLine("No saves found.");
                return;
            }

            var saveGameNumber = Path.GetFileName(latestSave);
            var game = games.FirstOrDefault(x => x.SaveGameNumber.ToString(CultureInfo.InvariantCulture).Equals(saveGameNumber)) ??
                       new Game(Convert.ToInt32(saveGameNumber));

            Console.WriteLine("{0} Game identified {1} for last save", DateTime.Now, game);

            var saveBackedUp = CheckIfSaveBackedUp(backupWatcher, game, latestSave);

            if (!saveBackedUp)
            {
                Console.WriteLine("{0} New save found at {1}", DateTime.Now, latestSave);
                var backupCreator = new BackupCreator(Configuration.BackupDirectory, BackupDirectoryResolver, FileOperationsHelper);
                backupCreator.CreateBackup(latestSave, game.Name);
            }
            Console.WriteLine("Done");
            Console.WriteLine();
        }

        private static bool CheckIfSaveBackedUp(BackupWatcher backupWatcher, Game game, string latestSave)
        {
            var latestBackupSave = backupWatcher.GetLatestGameSaveBackup(game.Name);

            bool isSaveBackedUp;
            if (string.IsNullOrEmpty(latestBackupSave))
            {
                isSaveBackedUp = false;
            }
            else
            {
                var comparer = new Comparer(new Crc16(), SystemDirectory, SystemFile);
                isSaveBackedUp = comparer.CompareDirectories(latestSave, latestBackupSave);
            }
            return isSaveBackedUp;
        }

        private static void BackupProcess()
        {
            var games = ReadGamesList();

            var period = Configuration.BackupPeriod;
            while (true)
            {
                FullBackupJob(games);
                Thread.Sleep(period);
            }
        }
    }
}
