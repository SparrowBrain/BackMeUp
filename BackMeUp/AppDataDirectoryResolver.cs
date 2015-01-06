namespace BackMeUp
{
    public class AppDataDirectoryResolver : BackupDirectoryResolver
    {
        public AppDataDirectoryResolver(string relativeLocation,  string backupDirectory) : base(relativeLocation, backupDirectory)
        {
        }

        protected override string BackupFolder
        {
            get
            {
                return "AppData";
            }
        }
    }
}