using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services.Configuration
{
    public class GameConfigurationReader : ConfigurationReader<GameConfiguration> {
        public GameConfigurationReader(IFile file) : base(file)
        {
        }
    }
}