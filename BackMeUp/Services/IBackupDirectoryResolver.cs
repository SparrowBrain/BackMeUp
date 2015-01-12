namespace BackMeUp.Services
{
    public interface IBackupDirectoryResolver
    {
        string GetSaveFilesBackupPath(string saveGameDirecotry, string timedGameDirectory);
        string GetLatestSaveFilesBackupPath(string gameName);
        string GetNewTimedGameBackupPath(string gameName);
    }
}