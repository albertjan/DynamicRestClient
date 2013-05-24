namespace DRC.Interfaces
{
    using System.IO;

    public interface IBodySerializer
    {
        bool CanHandle(string contentType);

        Stream Serialize<T>(T inp);

        T Deserialize<T>(Stream stream);
    }
}