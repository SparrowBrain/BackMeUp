using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using BackMeUp.Wrappers;
using NLog;

namespace BackMeUp.Data.Services
{
    public class ConfigurationService
    {
        private IFile File { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ConfigurationService(IFile file)
        {
            File = file;
        }

        public Configuration Read(string configurationXml)
        {
            if (!File.Exists(configurationXml))
            {
                Logger.Warn("Configuration file does not exist: " + configurationXml);
                return null;
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

        public void Write(string configurationXml, Configuration configuration)
        {
            var writeStream = GetWriteStream(configurationXml);
            using (writeStream)
            {
                var serializer = new XmlSerializer(typeof (Configuration));
                var xmlWriterSettings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true};
                var xmlWriter = XmlWriter.Create(writeStream, xmlWriterSettings);
                serializer.Serialize(xmlWriter, configuration);
            }
        }

        protected virtual Stream GetWriteStream(string configurationXml)
        {
            return new FileStream(configurationXml, FileMode.Create);
        }
    }
}
