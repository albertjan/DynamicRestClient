namespace DRC.Defaults
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;

    using Interfaces;

    public class DefaultCachedStringTokenizer : IStringTokenizer
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<string>> CacheDict = new ConcurrentDictionary<string, IEnumerable<string>>();

        public IEnumerable<string> GetTokens(string input)
        {
            if (CacheDict.ContainsKey (input)) return CacheDict[input];
            var retval = new List<string>();
            var sb = new StringBuilder();
            var last = char.MinValue;
            foreach (char c in input)
            {
                if (char.IsLower(last) && char.IsUpper(c))
                {
                    retval.Add(sb.ToString());
                    sb.Clear();
                }
                sb.Append(c);
                last = c;
            }
            retval.Add(sb.ToString().ToLower());
            CacheDict.AddOrUpdate(input, retval, (s, enumerable) => enumerable);
            return retval;
        }
    }
}
