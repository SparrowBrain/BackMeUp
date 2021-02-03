using BackMeUp.Data;
using BackMeUp.Services;
using BackMeUp.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public BackupProcess(Configuration configuration, SaveWatcher saveWatcher, Comparer comparer, BackupCreator backupCreator, BackupWatcher backupWatcher)
        {
            _configuration = configuration;
            _saveWatcher = saveWatcher;
            _comparer = comparer;
            _backupCreator = backupCreator;
            _backupWatcher = backupWatcher;
        }

        private void FullBackupJob(IEnumerable<Game> games)
        {
            Console.WriteLine("{1}{0}-------------------{0}Job started", Environment.NewLine, DateTime.Now);

            var latestSave = _saveWatcher.GetLatestSaveFilesPath();
            if (string.IsNullOrEmpty(latestSave))
            {
                Console.WriteLine("No saves found.");
                return;
            }

            var saveGameNumber = Path.GetFileName(latestSave);
            var game = games.FirstOrDefault(x => x.SaveGameNumber.ToString(CultureInfo.InvariantCulture).Equals(saveGameNumber)) ??
                       new Game(Convert.ToInt32(saveGameNumber));

            Console.WriteLine("{0} Game identified {1} for last save", DateTime.Now, game);

            var saveBackedUp = CheckIfSaveBackedUp(game, latestSave);

            if (!saveBackedUp)
            {
                Console.WriteLine("{0} New save found at {1}", DateTime.Now, latestSave);
                _backupCreator.CreateBackup(latestSave, game.Name);
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

        public async Task Run(List<Game> games)
        {
            var period = _configuration.BackupPeriod;
            while (true)
            {
                FullBackupJob(games);
                await Task.Delay(period);
            }
        }
    }
}