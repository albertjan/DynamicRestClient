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

        string EchoTest(object paramters);
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

            me.EchoTest.In = new Func<WebResponse, string> (wr =>
            {
                using (var sr = new StreamReader (wr.GetResponseStream ()))
                {
                    return sr.ReadToEnd ();
                }
            });

            IAmAFlickr client = Impromptu.ActLike<IAmAFlickr>(me);
            Console.WriteLine ("---------------------- getinfoonuser");
            Console.WriteLine (client.GetInfoOnPeople (new { user_id = "61304303%40N08" }));
            Console.WriteLine ("---------------------- echotest");
            Console.WriteLine (client.EchoTest (new { i_am_a = "banana", and_the_world_should_be_square = true }));
            Console.ReadLine();
        }
    }
}
