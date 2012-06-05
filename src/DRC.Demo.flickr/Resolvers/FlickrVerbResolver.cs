namespace DRC.Demo.flickr.Resolvers
{
    using Interfaces;

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
