namespace DRC.Interfaces
{
    public interface IUriComposer
    {
        string ComposeUri(string baseUri, string location, object[] functionParameters, object query);
    }
}
