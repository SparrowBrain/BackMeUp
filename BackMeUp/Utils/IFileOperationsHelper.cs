namespace BackMeUp.Utils
{
    public interface IFileOperationsHelper
    {
        void CreateDirectoryIfNotExists(string path);
        void CopyDirectory(string sourcePath, string destinationPath);
    }
}
