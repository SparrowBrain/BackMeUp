namespace BackMeUp
{
    public class ProgramFilesDirectoryResolver : BackupDirectoryResolver
    {
        public ProgramFilesDirectoryResolver(string relativeLocation, string backupDirectory)
            : base(relativeLocation, backupDirectory)
        {
        }

        protected override string BackupFolder
        {
            get
            {
                return "ProgramFiles";
            }
        }
    }
}