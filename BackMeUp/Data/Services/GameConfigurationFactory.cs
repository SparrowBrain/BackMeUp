using BackMeUp.Wrappers;

namespace BackMeUp.Data.Services
{
    public class GameConfigurationFactory:ConfigurationFactory<GameConfiguration>
    {
        protected override string ConfigurationFile
        {
            get { return "Data\\Games.xml"; }
        }

        protected override IConfigurationReader<GameConfiguration> GetConfigurationReader()
        {
            return new GameConfigurationReader(new SystemFile());
        }

        protected override GameConfiguration GetDefaultConfiguration()
        {
            return new GameConfiguration();
        }
    }
}