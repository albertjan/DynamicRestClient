namespace DRC
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Response object.
    /// </summary>
    public class Response : DynamicObject
    {
        private readonly RESTClient _client;
        
        public int StatusCode { get; set; }
        public Dictionary<string, string[]> Headers { get; set; }
        public Stream ResponseStream { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string ContentEncoding { get; set; }
        public string ResponseUri { get; set; }

        public Response(RESTClient client)
        {
            _client = client;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(Stream))
            {
                result = ResponseStream;
                return result != null || base.TryConvert(binder, out result);
            }

            if (binder.Type == typeof(int))
            {
                result = StatusCode;
                return true;
            }

            var me = typeof(RESTClient).GetMethod("GetDeserializationMethod", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(binder.Type);

            result = me.Invoke(_client, new object[] { this });

            return result != null || base.TryConvert(binder, out result);
        }
    }
}