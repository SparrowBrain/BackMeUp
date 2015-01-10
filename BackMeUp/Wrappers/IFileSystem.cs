using System.IO;

namespace BackMeUp.Wrappers
{
    public interface IFileSystem
    {
        void CreateDirectoryIfNotExists(string path);

        DirectoryInfo DirectoryCreateDirectory(string path);
        bool DirectoryExists(string path);
        string[] DirectoryGetFiles(string path);
        void FileCopy(string sourceFileName, string destFileName);
        void CopyDirectory(string sourceDirectory, string destinationDirectory);
    }
}
