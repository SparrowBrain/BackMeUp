namespace BackMeUp
{
    public class Configuration
    {
        public string BackupDirectory { get; set; }
        public string AppDataDirectory { get; set; }
        public string ProgramFilesDirectory { get; set; }
        public string RelativeAppDataLocation { get; set; }
        public string RelativeProgramFilesLocation { get; set; }
    }
}