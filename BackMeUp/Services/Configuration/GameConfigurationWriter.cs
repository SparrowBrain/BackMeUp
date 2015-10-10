using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services.Configuration
{
    public class GameConfigurationWriter:ConfigurationWriter<GameConfiguration>
    {
        public GameConfigurationWriter(IFile file) : base(file)
        {
        }
    }
}