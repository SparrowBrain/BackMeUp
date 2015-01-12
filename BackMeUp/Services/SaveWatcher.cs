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

        public string GetLatestSaveFilesPath()
        {
            var directories = _fileSystem.DirectoryGetDirectories(_saveGameDirectory, "*", SearchOption.AllDirectories);
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
                var directoryLastWriteTime = _fileSystem.DirectoryGetLastWriteTime(gameSave);
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
