using System;
using System.IO;
using System.Linq;
using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services
{
    public class SaveWatcher
    {
        private readonly string _saveGameDirectory;
        private readonly IFileSystem _fileSystem;
        
        

        public SaveWatcher(Configuration configuration, IFileSystem fileSystem)
        {
            _saveGameDirectory = configuration.SaveGameDirectory;
            _fileSystem = fileSystem;
        }

        public string GetLatestSaveFilesDirecotry()
        {
            var directories = _fileSystem.DirectoryGetDirectories(_saveGameDirectory, "*", SearchOption.AllDirectories);
            if (directories.Length == 0)
            {
                return null;
            }

            var saveFilesDirectories = directories.Where(x => Path.GetFileName(x).All(char.IsDigit)).ToArray();
            if (saveFilesDirectories.Length == 0)
            {
                return null;
            }

            var lastWriteTime=DateTime.MinValue;
            string latestSave = null;
            foreach (var gameSave in saveFilesDirectories)
            {
                var directoryinfo = new DirectoryInfo(gameSave);
                if (directoryinfo.LastWriteTime > lastWriteTime)
                {
                    lastWriteTime = directoryinfo.LastWriteTime;
                    latestSave = gameSave;
                }
            }

            return latestSave;
        }
    }
}
