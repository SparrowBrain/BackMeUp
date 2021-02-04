using BackMeUp.Annotations;
using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using BackMeUp.Wrappers;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace BackMeUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private AppState _appState;
        private string _status;

        public MainWindow()
        {
            InitializeComponent();
        }

        public AppState AppState
        {
            get => _appState;
            set
            {
                if (value == _appState) return;
                _appState = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.Info("Starting up...");
            var configuration = ReadConfiguration();
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
            var games = ReadGamesList();

            var backupProcess = new BackupProcess(configuration, saveWatcher, comparer, backupCreator, backupWatcher, games);
            backupProcess.SaveBackedUp += BackupProcess_SaveBackedUp;
            backupProcess.NothingHappened += BackupProcess_NothingHappened;
            backupProcess.ErrorHappened += BackupProcess_ErrorHappened;
            Task.Run(async () =>
            {
                await backupProcess.Run();
            });
        }

        private void BackupProcess_SaveBackedUp(object sender, SaveBackedUpEventArgs e)
        {
            AppState = AppState.BackedUp;
            Status = $"BackMeUp - {e.DateTime} {e.Game} backed up";
        }

        private void BackupProcess_NothingHappened(object sender, EventArgs e)
        {
            AppState = AppState.Nothing;
            Status = "BackMeUp - nothing new";
        }

        private void BackupProcess_ErrorHappened(object sender, ErrorEventArgs e)
        {
            AppState = AppState.Error;
            Status = $"BackMeUp - {e.GetException().Message}";
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Logger.Info("Shutting down...");
            Application.Current.Shutdown();
        }

        private Configuration ReadConfiguration()
        {
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configFile));
            Logger.Debug($"Read configuration: {configuration}");
            return configuration;
        }

        private List<Game> ReadGamesList()
        {
            try
            {
                var gamesConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "games.json");
                var games = JsonConvert.DeserializeObject<GamesConfiguration>(File.ReadAllText(gamesConfigFile));
                Logger.Debug($"Read games configuration: {games}");
                return games.Games;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception occured while reading games configuration, continuing without games");
                return new List<Game>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}