using BackMeUp.Wrappers;

namespace BackMeUp.Data.Services
{
    public class GameConfigurationWriter:ConfigurationWriter<GameConfiguration>
    {
        public GameConfigurationWriter(IFile file) : base(file)
        {
        }
    }
}