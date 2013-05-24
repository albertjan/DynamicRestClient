namespace DRC.BodySerializers
{
    using System.IO;
    using System.Text;
    
    using DRC.Interfaces;

    public class JsonBodySerializer : IBodySerializer
    {
        public bool CanHandle(string contentType)
        {
            return contentType.Contains("json");
        }

        public Stream Serialize<T>(T inp)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(SimpleJson.SerializeObject(inp)));
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return SimpleJson.DeserializeObject<T>(sr.ReadToEnd());    
            }
        }
    }
}