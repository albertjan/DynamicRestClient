namespace DRC.Defaults
{
    using System.Linq;
    using System.Collections.Generic;

    using DRC.Interfaces;
    
    public class DefaultUriComposer : IUriComposer
    {
        private readonly IQueryStringResolver _queryStringResolver;

        public DefaultUriComposer(IQueryStringResolver queryStringResolver)
        {
            _queryStringResolver = queryStringResolver;
        }

        public string ComposeUri(string baseUri, string location, object[] functionParameters, object query)
        {
            var queryDictionary = _queryStringResolver.ResolveQueryDict(query);

            //Part 1 the basics http://thing.com/base/ + the nouns "/test"
            string part1;
            if (location.StartsWith("http"))
            {
                part1 = location;
            }
            else
            {
                part1 = (baseUri != null ? baseUri + "/" : "") + location;
            }
            //Part 2 the parameters passed to the function call that aren't needed for the
            //output editor.
            var part2 = functionParameters == null || !functionParameters.Any()
                            ? ""
                            : "/" + functionParameters.Aggregate((l, r) => l + "/" + r);
            //Part 3 the querystring
            var part3 = ""; 
            
            if (queryDictionary != null && queryDictionary.Any())
            {    
                part3 += "?";
                foreach (var element in queryDictionary)
                {
                    if (element.Equals (queryDictionary.Last ()))
                        part3 += element.Key + "=" + element.Value;
                    else
                        part3 += element.Key + "=" + element.Value + "&";
                }
            }
               
            return part1 + part2 + part3;
        }
    }
}