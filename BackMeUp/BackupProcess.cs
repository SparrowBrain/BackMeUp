using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BackMeUp
{
    internal class BackupProcess
    {
        private readonly Configuration _configuration;
        private readonly SaveWatcher _saveWatcher;
        private readonly Comparer _comparer;
        private readonly BackupCreator _backupCreator;
        private readonly BackupWatcher _backupWatcher;
        private readonly GameName _gameName;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BackupProcess(Configuration configuration, SaveWatcher saveWatcher, Comparer comparer,
            BackupCreator backupCreator, BackupWatcher backupWatcher, GameName gameName)
        {
            _configuration = configuration;
            _saveWatcher = saveWatcher;
            _comparer = comparer;
            _backupCreator = backupCreator;
            _backupWatcher = backupWatcher;
            _gameName = gameName;
        }

        public event EventHandler<SaveBackedUpEventArgs> SaveBackedUp;

        public event EventHandler NothingHappened;

        public event ErrorEventHandler ErrorHappened;

        private void FullBackupJob()
        {
            Logger.Info("-------------------Job started-------------------");

            var latestSave = _saveWatcher.GetLatestSaveFilesPath();
            if (string.IsNullOrEmpty(latestSave))
            {
                Logger.Info("No saves found.");
                return;
            }

            var saveGameId = Path.GetFileName(latestSave);
            var gameName = _gameName.Resolve(Convert.ToInt32(saveGameId));

            Logger.Info($"Game identified {gameName} for last save");

            var isSaveBackedUp = CheckIfSaveBackedUp(gameName, latestSave);
            if (isSaveBackedUp)
            {
                Logger.Info("Already backed up");
                OnNothingHappened();
            }
            else
            {
                Logger.Info($"New save found at {latestSave}");
                _backupCreator.CreateBackup(latestSave, gameName);
                OnSaveBackedUp(new SaveBackedUpEventArgs { Game = gameName, DateTime = DateTime.Now });
            }

            Logger.Info("-------------------Job's done-------------------");
        }

        private bool CheckIfSaveBackedUp(string gameName, string latestSave)
        {
            var latestBackupSave = _backupWatcher.GetLatestGameSaveBackup(gameName);

            var isSaveBackedUp = !string.IsNullOrEmpty(latestBackupSave) && _comparer.CompareDirectoriesSame(latestSave, latestBackupSave);
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