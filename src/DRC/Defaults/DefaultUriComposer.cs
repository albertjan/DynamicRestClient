namespace DRC.Defaults
{
    using System.Linq;
    using System.Collections.Generic;

    using DRC.Interfaces;
    
    public class DefaultUriComposer : IUriComposer
    {
        public string ComposeUri(string baseUri, string location, object[] functionParameters, IEnumerable<KeyValuePair<string, string>> queryDictionary)
        {
            //Part 1 the basics http://thing.com/base/ + the nouns "/test"
            var part1 = (baseUri != null ? baseUri + "/" : "") + location;
            //Part 2 the parameters passed to the function call that aren't needed for the
            //output editor.
            var part2 = functionParameters == null || functionParameters.Count() == 0
                            ? ""
                            : "/" + functionParameters.Aggregate((l, r) => l + "/" + r);
            //Part 3 the querystring
            var part3 = ""; 
            
            if (queryDictionary != null && queryDictionary.Count () > 0)
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