namespace DRC.Interfaces
{
    using System.Collections.Generic;

    public interface IStringTokenizer
    {
        IEnumerable<string> GetTokens(string input);
    }
}