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
        T GetInfoOnPeople<T>(object query) where T: rsp;

        string EchoTest(object paramters);
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
                    new TypeRegistration { RegistrationType = typeof (IVerbResolver), InstanceType = typeof (FlickrVerbResolver) },
                    new TypeRegistration { RegistrationType = typeof (IQueryStringResolver), InstanceType = typeof (FlickrQueryStringResolver) }
                };
            } 
        }
        
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get { return Enumerable.Empty<InstanceRegistration>(); } }
        public IEnumerable<CollectionRegistration> CollectionRegistration { get { return Enumerable.Empty<CollectionRegistration>(); } }
    }

    class Program
    {
        static void Main (string[] args)
        {
            dynamic me = new RESTClient();
            
            me.Url = "http://api.flickr.com";

            me.OutputPipeLine.Add(1, new Tuple<string, Action<Request>>("addapikey", r => r.Uri = r.Uri + "&api_key=0936270ae439d42bce22ee3be8703112"));
            
            IAmAFlickr client = Impromptu.ActLike<IAmAFlickr>(me);
            Console.WriteLine ("---------------------- getinfoonpeople");
            Console.WriteLine (client.GetInfoOnPeople<rsp>(new { user_id = "61304303%40N08"}));
            Console.WriteLine ("---------------------- echotest");
            Console.WriteLine (client.EchoTest(new { i_am_a = "banana", and_the_world_should_be_square = true }));
            Console.ReadLine  ();
        }
    }
}
 