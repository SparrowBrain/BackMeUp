using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services.Configuration
{
    public class MainConfigurationWriter:ConfigurationWriter<MainConfiguration>
    {
        public MainConfigurationWriter(IFile file) : base(file)
        {
        }
    }
}