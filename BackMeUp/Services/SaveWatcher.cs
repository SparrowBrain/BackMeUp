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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDirectory _directory;
        private readonly string _saveGameDirectory;

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
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("{0}: {1}", "GetLatestSaveFilesPath()",
                        "Could not find any subdirectories for savegames");
                }
                return null;
            }

            // TODO: Shouldn't I be more specific about searching for folders inside user folders?
            var saveFilesDirectories = directories.Where(x => Path.GetFileName(x).All(char.IsDigit)).ToArray();
            if (saveFilesDirectories.Length == 0)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("{0}: {1}", "GetLatestSaveFilesPath()",
                        "Could not find any saved game file directories");
                }
                return null;
            }

            var lastWriteTime = DateTime.MinValue;
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

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("{0}: Latest save file path found={1}, LastWriteTime={2}", "GetLatestSaveFilesPath()",
                    latestSave, lastWriteTime);
            }

            return latestSave;
        }
    }
}