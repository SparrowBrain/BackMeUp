using System;

namespace BackMeUp.Data
{
    public class MainConfiguration
    {
        public MainConfiguration(string backupDirectory, string saveGamesDirectory, TimeSpan backupPeriod)
        {
            BackupDirectory = backupDirectory;
            SaveGamesDirectory = saveGamesDirectory;
            BackupPeriod = backupPeriod;
        }

        public string BackupDirectory { get; }
        
        public string SaveGamesDirectory { get; }

        public TimeSpan BackupPeriod { get; }
        
        protected bool Equals(MainConfiguration other)
        {
            return string.Equals(BackupDirectory, other.BackupDirectory) &&
                   string.Equals(SaveGamesDirectory, other.SaveGamesDirectory) &&
                   BackupPeriod.Equals(other.BackupPeriod);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (BackupDirectory != null ? BackupDirectory.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (SaveGamesDirectory != null ? SaveGamesDirectory.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ BackupPeriod.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("BackupDirectory=[{0}], SaveGamesDirectory=[{1}], BackupPeriod=[{2}]",
                BackupDirectory, SaveGamesDirectory, BackupPeriod);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MainConfiguration) obj);
        }
    }
}