namespace DRC.Demo.flickr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using DRC.Interfaces;
    using Resolvers;
    
    using ImpromptuInterface;

    public interface IAmAFlickr
    {
        T Do<T>(object query) where T: rsp;

        string Do(object paramters);
    }

    public class FlickrAppRegistration : IApplicationRegistration
    {
        public IEnumerable<TypeRegistration> TypeRegistrations 
        { 
            get
            {
                return new[]
                {
                    new TypeRegistration { RegistrationType = typeof (INounResolver), InstanceType = typeof (FlickrNounResolver) },
                    new TypeRegistration { RegistrationType = typeof (IVerbResolver), InstanceType = typeof (FlickrVerbResolver) }
                };
            } 
        }
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return Enumerable.Empty<InstanceRegistration>(); }
        }

        public IEnumerable<CollectionRegistration> CollectionRegistration { get { return Enumerable.Empty<CollectionRegistration>(); } }
    }

    class Program
    {
        private static string GetFlickrMethodCall(IEnumerable<string> tokens)
        {
            var retval = "";

            retval = "." + tokens.Last().ToLower();

            var tokensMinusLastMinusOn = tokens.Take(tokens.Count() - 1).Where(t => !string.Equals(t, "on", StringComparison.OrdinalIgnoreCase));

            foreach (var token in tokensMinusLastMinusOn)
            {
                if (token == tokensMinusLastMinusOn.Last() && tokens.Count() > 2)
                    retval += token;
                else
                    retval += "." + token.ToLower();
            }
            return retval;
        }

        static void Main (string[] args)
        {
            dynamic me = new RESTClient();
            
            me.Url = "http://api.flickr.com";

            me.OutputPipeLine.Add(1,
                                  new Tuple<string, Action<Request>>("addapikey",
                                                                     request =>
                                                                     {
                                                                         request.Uri = request.Uri +
                                                                                       "&api_key=0936270ae439d42bce22ee3be8703112";
                                                                         Console.WriteLine(request.Uri);
                                                                     }));

            IAmAFlickr client = Impromptu.ActLike<IAmAFlickr>(me);
            Console.WriteLine ("---------------------- getinfoonpeople");
            Console.WriteLine (client.Do<rsp>(new { user_id = "61304303%40N08", method = "flickr" + GetFlickrMethodCall(me.Container.Resolve<IStringTokenizer>().GetTokens("GetInfoOnPeople")) }));
            Console.WriteLine ("---------------------- echotest");
            Console.WriteLine (client.Do(new { i_am_a = "banana", and_the_world_should_be_square = true, method = "flickr" + GetFlickrMethodCall(me.Container.Resolve<IStringTokenizer>().GetTokens("EchoTest")) }));
            Console.ReadLine  ();
        }
    }
}
 