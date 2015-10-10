using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BackMeUp.Wrappers;

namespace BackMeUp.Services.Configuration
{
    public abstract class ConfigurationWriter<T>
    {
        private IFile File { get; set; }

        protected ConfigurationWriter(IFile file)
        {
            File = file;
        }

        protected virtual Stream GetWriteStream(string xmlFile)
        {
            return new FileStream(xmlFile, FileMode.Create);
        }

        public void Write(string xmlFile, T configuration)
        {
            var writeStream = GetWriteStream(xmlFile);
            using (writeStream)
            {
                var serializer = new XmlSerializer(typeof(T));
                var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
                var xmlWriter = XmlWriter.Create((Stream)writeStream, xmlWriterSettings);
                serializer.Serialize(xmlWriter, configuration);
            }
        }
    }
}