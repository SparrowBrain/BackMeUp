using System;

namespace BackMeUp.Data
{
    public class Configuration
    {
        public Configuration(string backupDirectory, TimeSpan backupPeriod)
        {
            BackupDirectory = backupDirectory;
            BackupPeriod = backupPeriod;
        }

        public string BackupDirectory { get; }
        
        public TimeSpan BackupPeriod { get; }
        
        protected bool Equals(Configuration other)
        {
            return string.Equals(BackupDirectory, other.BackupDirectory) &&
                   BackupPeriod.Equals(other.BackupPeriod);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (BackupDirectory != null ? BackupDirectory.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ BackupPeriod.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"BackupDirectory=[{BackupDirectory}], BackupPeriod=[{BackupPeriod}]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Configuration) obj);
        }
    }
}