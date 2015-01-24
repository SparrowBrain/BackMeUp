using System;

namespace BackMeUp.Data
{
    public class Configuration
    {
        public string BackupDirectory { get; set; }

        // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Uplay
        // InstallLocation
        public string SaveGamesDirectory { get; set; }
        public TimeSpan BackupPeriod { get; set; }
    }
}