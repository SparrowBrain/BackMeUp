namespace BackMeUp.Services
{
    public interface IBackupDirectoryResolver
    {
        string GetSaveFilesBackupPath(string gameSaveFilesDirecotry, string timedGameDirectory);
        string GetLatestSaveFilesBackupPath(string gameName);
        string GetNewTimedGameBackupPath(string gameName);
    }
}