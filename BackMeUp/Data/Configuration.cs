using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace BackMeUp.Data
{
    [Serializable]
    public class Configuration
    {
        public string BackupDirectory { get; set; }

        // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Uplay
        // InstallLocation
        public string SaveGamesDirectory { get; set; }

        [XmlIgnore]
        public TimeSpan BackupPeriod { get; set; }

        [DefaultValue(0)]
        public int BackupPeriodSeconds
        {
            get { return (int) BackupPeriod.TotalSeconds; }
            set { BackupPeriod = TimeSpan.FromSeconds(value); }
        }

        public List<Game> GameList { get; set; }

        public override string ToString()
        {
            var gameListBuilder = new StringBuilder();
            foreach (var game in GameList)
            {
                gameListBuilder.AppendFormat("{0};", game);
            }
            return string.Format("BackupDirectory=[{0}], SaveGamesDirectory=[{1}], BackupPeriod=[{2}], GameList=[{3}]",
                BackupDirectory, SaveGamesDirectory, BackupPeriod, gameListBuilder);
        }


    }
}