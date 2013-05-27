namespace DRC.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IUriComposer
    {
        string ComposeUri(string baseUri, string location, object[] functionParameters, object query);
    }
}
