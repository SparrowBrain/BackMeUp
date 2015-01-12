using System;
using System.IO;
using System.Linq;
using BackMeUp.Data;
using BackMeUp.Wrappers;
using NLog;

namespace BackMeUp.Services
{
    public class SaveWatcher
    {
        private readonly string _saveGameDirectory;
        private readonly IDirectory _directory;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        
        

        public SaveWatcher(Configuration configuration, IDirectory directory)
        {
            _saveGameDirectory = configuration.SaveGameDirectory;
            _directory = directory;
        }

        public string GetLatestSaveFilesPath()
        {
            var directories = _directory.GetDirectories(_saveGameDirectory, "*", SearchOption.AllDirectories);
            if (directories.Length == 0)
            {
                
                return null;
            }

            // TODO: Shouldn't I be more specific about searching for folders inside user folders?
            var saveFilesDirectories = directories.Where(x => Path.GetFileName(x).All(char.IsDigit)).ToArray();
            if (saveFilesDirectories.Length == 0)
            {
                return null;
            }

            var lastWriteTime=DateTime.MinValue;
            string latestSave = null;
            foreach (var gameSave in saveFilesDirectories)
            {
                var directoryLastWriteTime = _directory.GetLastWriteTime(gameSave);
                if (directoryLastWriteTime > lastWriteTime)
                {
                    lastWriteTime = directoryLastWriteTime;
                    latestSave = gameSave;
                }
            }

            return latestSave;
        }
    }
}
