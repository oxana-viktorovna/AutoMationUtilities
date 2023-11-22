using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SharedCore.FileUtilities
{
    public static class XmlWorker
    {
        public static T ReadFromFile<T>(string filePath)
        {
            var xmlFile = File.ReadAllText(filePath);
            var xml = DeserializeFromStringReader<T>(xmlFile);

            return xml;
        }

        public static T DeserializeXmlFromMemoryStream<T>(string input)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                try
                {
                    var runResult = serializer.Deserialize(reader);

                    return (T)runResult;
                }
                catch (InvalidOperationException ex)
                {
                    return default;
                }
            }
        }

        public static T DeserializeFromStringReader<T>(string input)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using var stringReader = new StringReader(input);
            var xmlReader = new XmlTextReader(stringReader);

            return (T)xmlSerializer.Deserialize(xmlReader);
        }
    }
}
