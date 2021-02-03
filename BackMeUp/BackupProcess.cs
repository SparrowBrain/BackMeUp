using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace BackMeUp
{
    internal class BackupProcess
    {
        private readonly Configuration _configuration;
        private readonly SaveWatcher _saveWatcher;
        private readonly Comparer _comparer;
        private readonly BackupCreator _backupCreator;
        private readonly BackupWatcher _backupWatcher;
        private readonly List<Game> _games;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BackupProcess(Configuration configuration, SaveWatcher saveWatcher, Comparer comparer,
            BackupCreator backupCreator, BackupWatcher backupWatcher, List<Game> games)
        {
            _configuration = configuration;
            _saveWatcher = saveWatcher;
            _comparer = comparer;
            _backupCreator = backupCreator;
            _backupWatcher = backupWatcher;
            _games = games;
        }

        public event EventHandler<SaveBackedUpEventArgs> SaveBackedUp;

        public event EventHandler NothingHappened;

        public event ErrorEventHandler ErrorHappened;

        private void FullBackupJob()
        {
            Console.WriteLine("{1}{0}-------------------{0}Job started", Environment.NewLine, DateTime.Now);

            var latestSave = _saveWatcher.GetLatestSaveFilesPath();
            if (string.IsNullOrEmpty(latestSave))
            {
                Console.WriteLine("No saves found.");
                return;
            }

            var saveGameNumber = Path.GetFileName(latestSave);
            var game = _games.FirstOrDefault(x => x.SaveGameNumber.ToString(CultureInfo.InvariantCulture).Equals(saveGameNumber)) ??
                       new Game(Convert.ToInt32(saveGameNumber));

            Console.WriteLine("{0} Game identified {1} for last save", DateTime.Now, game);

            var isSaveBackedUp = CheckIfSaveBackedUp(game, latestSave);

            if (isSaveBackedUp)
            {
                OnNothingHappened();
            }
            else
            {
                Console.WriteLine("{0} New save found at {1}", DateTime.Now, latestSave);
                _backupCreator.CreateBackup(latestSave, game.Name);
                OnSaveBackedUp(new SaveBackedUpEventArgs { Game = game.Name, DateTime = DateTime.Now });
            }

            Console.WriteLine("Done");
            Console.WriteLine();
        }

        private bool CheckIfSaveBackedUp(Game game, string latestSave)
        {
            var latestBackupSave = _backupWatcher.GetLatestGameSaveBackup(game.Name);

            bool isSaveBackedUp;
            if (string.IsNullOrEmpty(latestBackupSave))
            {
                isSaveBackedUp = false;
            }
            else
            {
                isSaveBackedUp = _comparer.CompareDirectories(latestSave, latestBackupSave);
            }
            return isSaveBackedUp;
        }

        public async Task Run()
        {
            var period = _configuration.BackupPeriod;
            while (true)
            {
                try
                {
                    FullBackupJob();
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error during full backup job");
                    OnErrorHappened(new ErrorEventArgs(e));
                }

                await Task.Delay(period);
            }
        }

        protected virtual void OnSaveBackedUp(SaveBackedUpEventArgs eventArgs)
        {
            SaveBackedUp?.Invoke(this, eventArgs);
        }

        protected virtual void OnNothingHappened()
        {
            NothingHappened?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnErrorHappened(ErrorEventArgs e)
        {
            ErrorHappened?.Invoke(this, e);
        }
    }
}