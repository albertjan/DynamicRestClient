namespace DRC.ProtocolBuffers
{
    using System.IO;
    using DRC.Interfaces;

    public class ProtocolBuffersBodySerializer : IBodySerializer
    {
        public bool CanHandle(string contentType)
        {
            return contentType.Contains("protocol-buffers");
        }

        public Stream Serialize<T>(T inp)
        {
            var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, inp);
            return ms;
        }

        public T Deserialize<T>(Stream stream)
        {
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
    }
}