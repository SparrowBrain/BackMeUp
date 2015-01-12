using System.IO;
using BackMeUp.Wrappers;

namespace BackMeUp.Utils
{
    public class FileOperationsHelper : IFileOperationsHelper
    {
        private IFile SystemFile { get; set; }
        private IDirectory SystemDirectory { get; set; }

        public FileOperationsHelper(IFile systemFile, IDirectory systemDirectory)
        {
            SystemFile = systemFile;
            SystemDirectory = systemDirectory;
        }

        public void CreateDirectoryIfNotExists(string path)
        {
            if (!SystemDirectory.Exists(path))
            {
                SystemDirectory.CreateDirectory(path);
            }
        }

        public void CopyDirectory(string sourcePath, string destinationPath)
        {
            CreateDirectoryIfNotExists(destinationPath);

            foreach (var file in SystemDirectory.GetFiles(sourcePath))
            {
                var fileName = Path.GetFileName(file);
                SystemFile.Copy(file, Path.Combine(destinationPath, fileName));
            }
        }
    }
}