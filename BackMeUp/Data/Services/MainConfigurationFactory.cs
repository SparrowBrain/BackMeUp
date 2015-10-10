using BackMeUp.Wrappers;

namespace BackMeUp.Data.Services
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
    }
}