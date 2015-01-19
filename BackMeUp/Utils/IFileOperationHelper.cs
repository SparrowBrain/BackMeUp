namespace BackMeUp.Utils
{
    public interface IFileOperationHelper
    {
        void CreateDirectoryIfNotExists(string path);
        void CopyDirectory(string sourcePath, string destinationPath);
    }
}
