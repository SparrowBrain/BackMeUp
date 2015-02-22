using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BackMeUp.Wrappers;
using NLog;

namespace BackMeUp.Data.Services
{
    public class ConfigurationWriter
    {
        private IFile File { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ConfigurationWriter(IFile file)
        {
            File = file;
        }

        protected virtual Stream GetWriteStream(string configurationXml)
        {
            return new FileStream(configurationXml, FileMode.Create);
        }

        public void Write(string configurationXml, Configuration configuration)
        {
            var writeStream = GetWriteStream(configurationXml);
            using (writeStream)
            {
                var serializer = new XmlSerializer(typeof (Configuration));
                var xmlWriterSettings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true};
                var xmlWriter = XmlWriter.Create((Stream) writeStream, xmlWriterSettings);
                serializer.Serialize(xmlWriter, configuration);
            }
        }
    }
}