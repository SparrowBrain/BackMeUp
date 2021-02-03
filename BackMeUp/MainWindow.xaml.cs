using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using BackMeUp.Wrappers;
using Newtonsoft.Json;

namespace BackMeUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var configuration = new Configuration(
                "E:\\Backup",
                TimeSpan.FromMinutes(10));
            var uPlayPathResolver = new UPlayPathResolver();
            var saveGamesDirectory = Path.Combine(uPlayPathResolver.GetUPlayInstallationDirectory(), Constants.SaveGames);
            var systemDirectory = new SystemDirectory();
            var systemFile = new SystemFile();
            var fileOperationHelper = new FileOperationHelper(systemFile, systemDirectory);
            var directoryNameFixer = new DirectoryNameFixer();
            var backupDirectoryResolver = new BackupDirectoryResolver(configuration.BackupDirectory, systemDirectory, directoryNameFixer);
            var saveWatcher = new SaveWatcher(saveGamesDirectory, systemDirectory);
            var crc16 = new Crc16();
            var comparer = new Comparer(crc16, systemDirectory, systemFile);
            var backupCreator = new BackupCreator(configuration.BackupDirectory, backupDirectoryResolver, fileOperationHelper);
            var backupWatcher = new BackupWatcher(backupDirectoryResolver);

            var backupProcess = new BackupProcess(configuration, saveWatcher, comparer, backupCreator, backupWatcher);
            Task.Run(async()=>
            {
                var games = ReadGamesList();
                await backupProcess.Run(games);
            });
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private List<Game> ReadGamesList()
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
    }
}
