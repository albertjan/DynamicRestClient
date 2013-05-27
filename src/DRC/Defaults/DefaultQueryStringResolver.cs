namespace DRC.Defaults
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Interfaces;

    public class DefaultQueryStringResolver : IQueryStringResolver
    {
        private IStringTokenizer Tokenizer { get; set; }

        public DefaultQueryStringResolver(IStringTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public IEnumerable<KeyValuePair<string, string>> ResolveQueryDict (object anonymousQueryObject)
        {
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
    }
}