using System.IO;
using System.Xml.Serialization;
using BackMeUp.Wrappers;

namespace BackMeUp.Data.Services
{
    public abstract class ConfigurationReader<T> : IConfigurationReader<T>
    {
        private IFile File { get; set; }

        protected ConfigurationReader(IFile file)
        {
            File = file;
        }

        public T Read(string configurationXml)
        {
            if (!File.Exists(configurationXml))
            {
                throw new FileNotFoundException("File does not exist", configurationXml);
            }

            var readStream = GetReadStream(configurationXml);
            using (readStream)
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(readStream);
            }
        }

        protected virtual Stream GetReadStream(string configurationXml)
        {
            return new FileStream(configurationXml, FileMode.Open);
        }
    }
}