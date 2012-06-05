using System.Collections.Generic;

namespace DRCSharedInterfaces
{
    public interface IStringTokenizer
    {
        IEnumerable<string> GetTokens(string input);
    }
}