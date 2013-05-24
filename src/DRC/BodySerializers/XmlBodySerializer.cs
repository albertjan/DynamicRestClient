namespace DRC.BodySerializers
{
    using System.IO;
    using System.Xml.Serialization;

    using DRC.Interfaces;
    
    public class XmlBodySerializer : IBodySerializer
    {
        public bool CanHandle(string contentType)
        {
            return contentType.Contains("xml");
        }

        public Stream Serialize<T>(T inp)
        {
            var ms = new MemoryStream();
            new XmlSerializer(typeof(T)).Serialize(ms, inp);
            return ms;
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T) new XmlSerializer(typeof (T)).Deserialize(stream);
        }
    }
}