using BackMeUp.Wrappers;

namespace BackMeUp.Data.Services
{
    public class GameConfigurationReader : ConfigurationReader<GameConfiguration> {
        public GameConfigurationReader(IFile file) : base(file)
        {
        }
    }
}