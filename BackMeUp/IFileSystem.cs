using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackMeUp
{
    public interface IFileSystem
    {
        void CreateDirectoryIfNotExists(string path);

        DirectoryInfo DirectoryCreateDirectory(string path);
        bool DirectoryExists(string path);
        string[] DirectoryGetFiles(string path);

        void FileCopy(string sourceFileName, string destFileName);

    }

    public class FileSystem:IFileSystem
    {
        public void CreateDirectoryIfNotExists(string path)
        {
            if (!DirectoryExists(path))
            {
                DirectoryCreateDirectory(path);
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
