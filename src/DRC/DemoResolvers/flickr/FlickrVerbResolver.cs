using System;
using System.Linq;
using DRCSharedInterfaces;

namespace DRC.DemoResolvers.flickr
{
    public class FlickrVerbResolver : IVerbResolver
    {
        public IStringTokenizer Tokenizer { get; set; }

        public FlickrVerbResolver (IStringTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public Verb ResolveVerb (string functionName)
        {
            //afaik flickr only has get methods
            return Verb.GET;
        }
    }
}
