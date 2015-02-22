using System.IO;
using System.Xml.Serialization;
using BackMeUp.Wrappers;
using NLog;

namespace BackMeUp.Data.Services
{
    public interface IConfigurationReader
    {
        Configuration Read(string configurationXml);
    }

    public class ConfigurationReader : IConfigurationReader
    {
        private IFile File { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ConfigurationReader(IFile file)
        {
            File = file;
        }

        public Configuration Read(string configurationXml)
        {
            if (!File.Exists(configurationXml))
            {
                throw new FileNotFoundException("File does not exist", configurationXml);
            }

            var readStream = GetReadStream(configurationXml);
            using (readStream)
            {
                var serializer = new XmlSerializer(typeof(Configuration));
                return (Configuration) serializer.Deserialize(readStream);
            }
        }

        protected virtual Stream GetReadStream(string configurationXml)
        {
            return new FileStream(configurationXml, FileMode.Open);
        }
    }
}
