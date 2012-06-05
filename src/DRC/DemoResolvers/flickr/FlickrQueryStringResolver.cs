namespace DRC.DemoResolvers.flickr
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Interfaces;

    public class FlickrQueryStringResolver : IQueryStringResolver
    {
        public IStringTokenizer Tokenizer { get; set; }
        public string ApiKey { get; set; }

        public FlickrQueryStringResolver(IStringTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public IEnumerable<KeyValuePair<string, string>> ResolveQueryDict (string functionName, object anonymousQueryObject)
        {
            yield return new KeyValuePair<string, string>("api_key", ApiKey);
            yield return new KeyValuePair<string, string>("method", "flickr" + GetFlickrMethodCall(Tokenizer.GetTokens(functionName)));
            var props = TypeDescriptor.GetProperties (anonymousQueryObject);
            foreach (PropertyDescriptor prop in props)
            {
                var val = prop.GetValue (anonymousQueryObject).ToString ();
                if (val != null)
                {
                    yield return new KeyValuePair<string, string> (prop.Name, val);
                }
            }
        }

        private static string GetFlickrMethodCall (IEnumerable<string> tokens)
        {
            var retval = "";
               
            foreach (var token in tokens)
            {
                if (token == tokens.Last ())
                    retval += token;
                else
                    retval += "." + token.ToLower();
            }
            return retval;
        }
    }
}