using System.Collections.Generic;

namespace DRCSharedInterfaces
{
    public interface IQueryStringResolver
    {
        IEnumerable<KeyValuePair<string, string>> ResolveQueryDict(string functionName, object anonymousQueryObject);
    }
}