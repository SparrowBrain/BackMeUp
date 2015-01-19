using System;
using System.IO;
using BackMeUp.Wrappers;

namespace BackMeUp.Utils
{
    public class FileOperationHelper : IFileOperationHelper
    {
        private IFile SystemFile { get; set; }
        private IDirectory SystemDirectory { get; set; }

        public FileOperationHelper(IFile systemFile, IDirectory systemDirectory)
        {
            SystemFile = systemFile;
            SystemDirectory = systemDirectory;
        }

        public void CreateDirectoryIfNotExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(path);
            }

            if (!SystemDirectory.Exists(path))
            {
                SystemDirectory.CreateDirectory(path);
            }
        }

        public void CopyDirectory(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException("sourcePath");
            }
            if (string.IsNullOrEmpty(destinationPath))
            {
                throw new ArgumentException("destinationPath");
            }

            CreateCopyDirectory(sourcePath, destinationPath);
        }

        private void CreateCopyDirectory(string sourcePath, string destinationPath)
        {
            CreateDirectoryIfNotExists(destinationPath);
            foreach (var file in SystemDirectory.GetFiles(sourcePath))
            {
                var fileName = Path.GetFileName(file);
                var destinationFile = Path.Combine(destinationPath, fileName);

                if (SystemDirectory.Exists(file))
                {
                    CreateCopyDirectory(file, destinationFile);
                }
                else
                {
                    SystemFile.Copy(file, destinationFile);
                }
            }
        }
    }
}