using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRCSharedInterfaces;

namespace DRC.Defaults
{
    public class DefaultCachedStringTokenizer : IStringTokenizer
    {
        private static readonly Dictionary<string, IEnumerable<string>> CacheDict = new Dictionary<string, IEnumerable<string>>();

        public IEnumerable<string> GetTokens(string input)
        {
            if (CacheDict.ContainsKey (input)) return CacheDict[input];
            var retval = new List<string>();
            var sb = new StringBuilder();
            char last = char.MinValue;
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
            retval.Add(sb.ToString());
            CacheDict.Add (input, retval);
            return retval;
        }
    }
}
