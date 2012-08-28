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
            if (PredefinedUrls.ContainsKey(naaca)) return PredefinedUrls[naaca];
            
            var tokens = Tokenizer.GetTokens (naaca).ToList();
            
            return tokens.Count > 1
                       ? tokens.Skip(1).Aggregate((s1, s2) => s1 + "/" + s2)
                       : (tokens.Count == 1 ? tokens[0] : "");
        }
    }
}