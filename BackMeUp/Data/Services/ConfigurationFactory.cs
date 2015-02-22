using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackMeUp.Wrappers;

namespace BackMeUp.Data.Services
{
    public class ConfigurationFactory
    {
        private const string ConfigurationFile = "Data\\configuration.xml";

        public Configuration GetConfiguration()
        {
            var configurationReader = GetConfigurationReader();
            try
            {
                var configuration = configurationReader.Read(ConfigurationFile);
                return configuration;
            }
            catch (Exception ex)
            {
                return new Configuration();
            }
        }

        protected virtual IConfigurationReader GetConfigurationReader()
        {
            return new ConfigurationReader(new SystemFile());
        }
    }
}
