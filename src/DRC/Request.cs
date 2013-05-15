namespace DRC
{
    using System.Collections.Generic;

    /// <summary>
    /// Request object.
    /// </summary>
    public class Request
    {
        public Request()
        {
            Headers = new Dictionary<string, string>();
        }

        public byte[] Body { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string,string> Headers { get; set; }
        public string Uri { get; set; }
    }
}