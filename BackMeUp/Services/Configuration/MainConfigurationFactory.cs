using System;
using System.IO;
using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services.Configuration
{
    public class MainConfigurationFactory:ConfigurationFactory<MainConfiguration>
    {
        protected override string ConfigurationFile
        {
            get { return "Data\\configuration.xml"; }
        }

        protected override IConfigurationReader<MainConfiguration> GetConfigurationReader()
        {
            return new MainConfigurationReader(new SystemFile());
        }

        protected override MainConfiguration GetDefaultConfiguration()
        {
            return new MainConfiguration()
            {
                BackupPeriod = new TimeSpan(0, 10, 0),
                BackupDirectory =
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                        @"Ubisoft\Ubisoft Game Launcher", Constants.SaveGames),
                SaveGamesDirectory =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Backup")
            };
        }
    }
}