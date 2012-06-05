namespace DRC.Interfaces
{
    using System.Collections.Generic;
    
    public interface IQueryStringResolver
    {
        IEnumerable<KeyValuePair<string, string>> ResolveQueryDict(string functionName, object anonymousQueryObject);
    }
}