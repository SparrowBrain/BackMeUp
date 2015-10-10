using BackMeUp.Data;
using BackMeUp.Wrappers;

namespace BackMeUp.Services.Configuration
{
    public class MainConfigurationReader : ConfigurationReader<MainConfiguration>
    {
        public MainConfigurationReader(IFile file) : base(file)
        {
        }
    }
}
