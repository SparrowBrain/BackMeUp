using System.IO;

namespace BackMeUp.Wrappers
{
    public class FileSystem:IFileSystem
    {
        public void CreateDirectoryIfNotExists(string path)
        {
            if (!DirectoryExists(path))
            {
                DirectoryCreateDirectory(path);
            }
        }
        
        public void CopyDirectory(string sourcePath, string destinationPath)
        {
            CreateDirectoryIfNotExists(destinationPath);

            foreach (var file in DirectoryGetFiles(sourcePath))
            {
                var fileName = Path.GetFileName(file);
                FileCopy(file, Path.Combine(destinationPath, fileName));
            }
        }

        #region Directory
        
        public DirectoryInfo DirectoryCreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string[] DirectoryGetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] DirectoryGetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] DirectoryGetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetDirectories(path, searchPattern, searchOption);
        }

        public string[] DirectoryGetFileSystemEntries(string path)
        {
            return Directory.GetFileSystemEntries(path);
        }

        #endregion Directory

        #region File

        public void FileCopy(string sourceFileName, string destFileName)
        {
            File.Copy(sourceFileName, destFileName);
        }

        #endregion File
    }
}