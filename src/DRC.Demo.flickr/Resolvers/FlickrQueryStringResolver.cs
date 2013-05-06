namespace DRC.Demo.flickr.Resolvers
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
                var val = prop.GetValue (anonymousQueryObject);
                if (val != null)
                {
                    yield return new KeyValuePair<string, string> (prop.Name, val.ToString ());
                }
            }
        }

        private static string GetFlickrMethodCall (IEnumerable<string> tokens)
        {
            var retval = "";

            retval = "." + tokens.Last ().ToLower ();

            var tokensMinusLastMinusOn = tokens.Take (tokens.Count () - 1).Where (t => t != "on");

            foreach (var token in tokensMinusLastMinusOn)
            {
                if (token == tokensMinusLastMinusOn.Last () && tokens.Count() > 2)
                    retval += token;
                else
                    retval += "." + token.ToLower();
            }
            return retval;
        }
    }
}