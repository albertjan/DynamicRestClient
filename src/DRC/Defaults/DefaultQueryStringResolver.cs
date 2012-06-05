using System;
using System.Collections.Generic;
using System.ComponentModel;
using DRCSharedInterfaces;

namespace DRC.Defaults
{
    public class DefaultQueryStringResolver : IQueryStringResolver
    {
        public IStringTokenizer Tokenizer { get; set; }

        public DefaultQueryStringResolver(IStringTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public IEnumerable<KeyValuePair<string, string>> ResolveQueryDict (string functionName, object anonymousQueryObject)
        {
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
    }
}