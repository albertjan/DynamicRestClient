using System;
using System.Linq;
using DRCSharedInterfaces;
using INounResolverDemoImplementations;

namespace DRC.Defaults
{
    public class DefaultNounResolver : INounResolver
    {
        public IStringTokenizer Tokenizer { get; set; }

        public DefaultNounResolver(IStringTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public string ResolveNoun(string naaca)
        {
            return Tokenizer.GetTokens (naaca).Skip(1).Aggregate((s1, s2) => s1 + "/" + s2);
        }
    }
}