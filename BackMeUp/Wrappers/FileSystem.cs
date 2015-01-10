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
        
        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            CreateDirectoryIfNotExists(destinationDirectory);

            foreach (var file in DirectoryGetFiles(sourceDirectory))
            {
                var fileName = Path.GetFileName(file);
                FileCopy(file, Path.Combine(destinationDirectory, fileName));
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

        #endregion Directory

        #region File

        public void FileCopy(string sourceFileName, string destFileName)
        {
            File.Copy(sourceFileName, destFileName);
        }

        #endregion File
    }
}