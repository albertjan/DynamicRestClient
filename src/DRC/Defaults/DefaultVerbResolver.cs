using System;
using System.Linq;
using DRCSharedInterfaces;

namespace DRC.Defaults
{
    public class DefaultVerbResolver : IVerbResolver
    {
        public IStringTokenizer Tokenizer { get; set; }

        public DefaultVerbResolver(IStringTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public Verb ResolveVerb(string functionName)
        {
            var verb = Tokenizer.GetTokens(functionName).FirstOrDefault();
            if (verb == null) throw new ArgumentException("A Functionname must have atleast one token.");
            switch (verb.ToLower())
            {
                case "get":
                    return Verb.GET;
                case "put":
                case "add":
                case "new":
                    return Verb.PUT;
                case "post":
                case "update":
                case "save":
                    return Verb.POST;
                case "delete":
                case "remove":
                case "del":
                case "kill":
                    return Verb.DELETE;
                default:
                    return Verb.GET;
            }
        }
    }
}
