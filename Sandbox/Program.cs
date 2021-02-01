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
using BackMeUp.Services.Configuration;
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

        private static readonly MainConfiguration Configuration = new MainConfiguration
        {
            BackupDirectory = "E:\\Backup",
            SaveGamesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Ubisoft\Ubisoft Game Launcher", Constants.SaveGames)
        };

        private static readonly Game[] _games =
        {
            new Game("Far Cry 3", 46),
            new Game("Far Cry 3 Blood Dragon", 205),
            new Game("Assasin's Creed IV Back Flag", 437),
            new Game("Tom Clancy's Rainbow Six Siege", 1843)
        };

        private static readonly IFile SystemFile = new SystemFile();
        private static readonly IDirectory SystemDirectory = new SystemDirectory();
        private static readonly IBackupDirectoryResolver BackupDirectoryResolver = new BackupDirectoryResolver(Configuration.BackupDirectory, SystemDirectory, new DirectoryNameFixer());
        private static readonly IFileOperationHelper FileOperationsHelper = new FileOperationHelper(SystemFile, SystemDirectory);
        

        static void Main(string[] args)
        {
            //WriteConfigurationXml();
            //WriteConfigurationGameList();

            //Backup();
            //Watcher();
            //BackupWatcher();
            //FullBackupJob();

            BackupProcess();
        }

        private static void WriteConfigurationXml()
        {
            var configurationService = new MainConfigurationWriter(new SystemFile());
            //var configuration = new MainConfiguration
            //{
            //    SaveGamesDirectory = @"C:\Saves",
            //    BackupDirectory = @"C:\Backup",
            //    BackupPeriod = TimeSpan.FromMinutes(10)
            //};

            var configuration = new MainConfigurationFactory().GetConfiguration();
            configurationService.Write("abc.xml", configuration);
        }

        private static void WriteConfigurationGameList()
        {
            var configurationService = new GameConfigurationWriter(new SystemFile());
            var configuration = new GameConfiguration
            {
                Games = new List<Game>(_games)
            };


            configurationService.Write("Games.xml", configuration);
        }

        private static List<Game> ReadGamesList()
        {
            try
            {
                var gameConfigurationReader = new GameConfigurationReader(new SystemFile());
                var games = gameConfigurationReader.Read("Games.xml").Games;

                return games;
            }
            catch
            {
                return _games.ToList();
            }
        }

        private static void FullBackupJob(List<Game> games)
        {
            Console.WriteLine("{1}{0}-------------------{0}Job started", Environment.NewLine, DateTime.Now);
            var saveWatcher = new SaveWatcher(Configuration.SaveGamesDirectory, SystemDirectory);
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

            bool saveBackedUp;
            if (string.IsNullOrEmpty(latestBackupSave))
            {
                saveBackedUp = false;
            }
            else
            {
                var comparer = new Comparer(new Crc16(), SystemDirectory, SystemFile);
                saveBackedUp = comparer.CompareDirectories(latestSave, latestBackupSave);
            }
            return saveBackedUp;
        }

        private static void BackupProcess()
        {
            var games = ReadGamesList();

            var period = new TimeSpan(0, 10, 0);
            while (true)
            {
                FullBackupJob(games);
                Thread.Sleep(period);
            }
        }
    }
}
