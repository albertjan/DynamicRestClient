namespace DRC.Demo.flickr
{
    using System;
    using System.IO;
    using System.Net;

    using Defaults;
    using Resolvers;
    
    using ImpromptuInterface;

    public interface IAmAFlickr
    {
        string GetInfoOnPeople (object query);
    }

    class Program
    {
        static void Main (string[] args)
        {
            var qst = new DefaultCachedStringTokenizer();
            dynamic me = new RESTClient(new FlickrNounResolver(qst), new FlickrQueryStringResolver(qst),
                                        new FlickrVerbResolver(qst), qst);

            me.Url = "http://api.flickr.com";

            me.QueryStringResolver.ApiKey = "0936270ae439d42bce22ee3be8703112";

            me.GetInfoOnPeople.In = new Func<WebResponse, string> (wr =>
            {
                using (var sr = new StreamReader (wr.GetResponseStream ()))
                {
                    return sr.ReadToEnd ();
                }
            });

            IAmAFlickr client = Impromptu.ActLike<IAmAFlickr>(me);

            Console.WriteLine (client.GetInfoOnPeople (new { user_id = "61304303%40N08" }));
            Console.ReadLine();
        }
    }
}
