using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BackMeUp.Data
{
    [Serializable]
    public class MainConfiguration
    {
        public string BackupDirectory { get; set; }
        
        public string SaveGamesDirectory { get; set; }

        [XmlIgnore]
        public TimeSpan BackupPeriod { get; set; }

        [DefaultValue(0)]
        public int BackupPeriodSeconds
        {
            get { return (int) BackupPeriod.TotalSeconds; }
            set { BackupPeriod = TimeSpan.FromSeconds(value); }
        }
        
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