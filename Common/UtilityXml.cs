using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common
{
    public static class UtilityXml
    {
        public static T DeserializeXml<T>(string data)
        {
            using (StringReader stringReader = new StringReader(data))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }

        public static T DeserializeXml<T>(string data, string rootElementName)
        {
            using (StringReader stringReader = new StringReader(data))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElementName));
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }

        public static async Task<T> DeserializeXmlAsync<T>(string data)
        {
            return await Task.Run(() => DeserializeXml<T>(data));
        }

        public static async Task<T> DeserializeXmlAsync<T>(string data, string rootElementName)
        {
            return await Task.Run(() => DeserializeXml<T>(data, rootElementName));
        }

        public static string SerializeXml<T>(object obj)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stringWriter, obj);

                return stringWriter.ToString();
            }
        }

        public static string SerializeXml<T>(object obj, string rootElementName)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElementName));
                xmlSerializer.Serialize(stringWriter, obj);

                return stringWriter.ToString();
            }
        }

        public static async Task<string> SerializeXmlAsync<T>(object obj)
        {
            return await Task.Run(() => SerializeXml<T>(obj));
        }

        public static async Task<string> SerializeXmlAsync<T>(object obj, string rootElementName)
        {
            return await Task.Run(() => SerializeXml<T>(obj, rootElementName));
        }
    }
}
