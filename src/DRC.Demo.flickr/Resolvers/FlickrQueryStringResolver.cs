namespace DRC.Demo.flickr.Resolvers
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using DRC.Interfaces;
    
    public class FlickrQueryStringResolver : IQueryStringResolver
    {
        private readonly IStringTokenizer _tokenizer;
        
        public FlickrQueryStringResolver(IStringTokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }
        
        private static string GetFlickrMethodCall(IEnumerable<string> tokens)
        {
            var retval = "flickr";

            retval += "." + tokens.Last().ToLower();

            var tokensMinusLastMinusOn = tokens.Take(tokens.Count() - 1).Where(t => !string.Equals(t, "on", StringComparison.OrdinalIgnoreCase));

            foreach (var token in tokensMinusLastMinusOn)
            {
                if (token == tokensMinusLastMinusOn.Last() && tokens.Count() > 2)
                    retval += token;
                else
                    retval += "." + token.ToLower();
            }
            return retval;
        }

        public IEnumerable<KeyValuePair<string, string>> ResolveQueryDict(object anonymousQueryObject, string functionName)
        {
            yield return new KeyValuePair<string, string>("method", GetFlickrMethodCall(_tokenizer.GetTokens(functionName)));

            var props = TypeDescriptor.GetProperties(anonymousQueryObject);
            foreach (PropertyDescriptor prop in props)
            {
                var val = prop.GetValue(anonymousQueryObject);
                if (val != null)
                {
                    yield return new KeyValuePair<string, string>(prop.Name, val.ToString());
                }
            }
        }
    }
}