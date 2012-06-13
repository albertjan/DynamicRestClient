namespace DRC.Defaults
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;

    public class DefaultNounResolver : INounResolver
    {
        public IStringTokenizer Tokenizer { get; set; }

        public Dictionary<string, string> PredefinedUrls { get; set; }

        public DefaultNounResolver(IStringTokenizer tokenizer)
        {
            PredefinedUrls = new Dictionary<string, string>();
            Tokenizer = tokenizer;
        }

        public string ResolveNoun(string naaca)
        {
            return PredefinedUrls.ContainsKey(naaca) ? PredefinedUrls[naaca] : Tokenizer.GetTokens (naaca).Skip(1).Aggregate((s1, s2) => s1 + "/" + s2);
        }
    }
}