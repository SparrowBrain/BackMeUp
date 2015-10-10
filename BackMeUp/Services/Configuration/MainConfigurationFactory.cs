using System;
using System.IO;
using BackMeUp.Data;
using BackMeUp.Utils;
using BackMeUp.Wrappers;

namespace BackMeUp.Services.Configuration
{
    public class MainConfigurationFactory : ConfigurationFactory<MainConfiguration>
    {
        protected override string ConfigurationFile
        {
            get { return "Data\\configuration.xml"; }
        }

        protected override IConfigurationReader<MainConfiguration> GetConfigurationReader()
        {
            return new MainConfigurationReader(new SystemFile());
        }

        protected virtual IUPlayPathResolver GetUPlayPathResolver()
        {
            return new UPlayPathResolver();
        }

        protected override MainConfiguration GetDefaultConfiguration()
        {
            var uPlayInstallationDirectory = GetUPlayPathResolver().GetUPlayInstallationDirectory();

            var defaultConfiguration = new MainConfiguration
            {
                BackupPeriod = new TimeSpan(0, 10, 0),
                SaveGamesDirectory = Path.Combine(uPlayInstallationDirectory, Constants.SaveGames),
                BackupDirectory =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "SparrowBrain\\BackMeUp\\Backup")
            };
            return defaultConfiguration;
        }
    }
}