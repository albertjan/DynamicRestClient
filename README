# DynamicRestClient #


A simple REST client. Using Dynamics.

```C#
//New up a new client
dynamic me = new RESTClient ();

//Set the url
me.Url = "http://someapi.com";

//Set an input editor for a route
me.In.GetThings = new Func<WebResponse, IEnumerable<Thing>> ( wr => {
	//fictive
	JSON.Deserialize<IEnumerable<Thing>>(wr.GetResponseStream());
});

//create an iterface to make it place nice again
interface IAmAThingGetter
{
	IEnumerable<Thing> GetThings();
}


//makes client behave typesafe
IAmAThingGetter client = me.ActLike<IAmAThingGetter>();

//var is an IEnumerable<Thing> here.
var things = client.GetThings();

```

Isn't it cute. More to come see the tests for more usages.

