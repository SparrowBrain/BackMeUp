using System.IO;

namespace BackMeUp.Wrappers
{
    public interface IFileSystem
    {
        void CreateDirectoryIfNotExists(string path);
        void CopyDirectory(string sourceDirectory, string destinationDirectory);

        DirectoryInfo DirectoryCreateDirectory(string path);
        bool DirectoryExists(string path);
        string[] DirectoryGetDirectories(string path);
        string[] DirectoryGetDirectories(string path, string searchPattern, SearchOption searchOption);
        string[] DirectoryGetFiles(string path);
        string[] DirectoryGetFileSystemEntries(string path);

        void FileCopy(string sourceFileName, string destFileName);
    }
}
