using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Common
{
    public static class SerializationHelper
    {
        static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers = new ConcurrentDictionary<Type, DataContractSerializer>();

        public static XmlDocument SerializeToXmlDocument(object obj)
        {
            Contract.Requires(obj != null);

            using (var stream = new MemoryStream())
            {
                var ser = GetSerializer(obj.GetType());
                ser.WriteObject(stream, obj);
                var res = new XmlDocument();
                res.LoadXml(Encoding.UTF8.GetString(stream.GetBuffer()));
                return res;
            }
        }

        public static byte[] SerializeToBytes(object obj)
        {
            Contract.Requires(obj != null);

            var xml = SerializeToXmlDocument(obj);
            return Encoding.UTF8.GetBytes(xml.OuterXml);
        }

        public static T Deserialize<T>(XmlDocument doc)
        {
            return (T)Deserialize(typeof(T), doc);
        }

        public static T Deserialize<T>(byte[] data)
        {
            Contract.Requires(data != null);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(Encoding.UTF8.GetString(data));

            return Deserialize<T>(xmlDocument);
        }

        public static object Deserialize(Type type, XmlDocument doc)
        {
            Contract.Requires(type != null);
            Contract.Requires(doc != null);

            using (var stream = new XmlNodeReader(doc))
                return GetSerializer(type).ReadObject(stream);
        }

        private static DataContractSerializer GetSerializer(Type type)
        {
            return Serializers.GetOrAdd(type, new DataContractSerializer(type));
        }
    }
}
