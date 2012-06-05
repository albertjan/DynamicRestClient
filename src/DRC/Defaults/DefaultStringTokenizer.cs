namespace DRC.Defaults
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Interfaces;

    public class DefaultStringTokenizer : IStringTokenizer
    {
        public IEnumerable<string> GetTokens(string input)
        {
            var sb = new StringBuilder();
            char last = char.MinValue;
            foreach (char c in input)
            {
                if (char.IsLower(last) && char.IsUpper(c))
                {
                    var token = sb.ToString ();
                    sb.Clear ();
                    yield return token;
                }
                sb.Append(c);
                last = c;
            }
            if (!String.IsNullOrWhiteSpace (sb.ToString ()))
            {
                yield return sb.ToString();
            }
        }
    }
}
