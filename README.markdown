# DynamicRestClient #

A simple REST client. Using Dynamics.

Lets asume there is a api at someapi.com which has a route `/things` to which you can do a get request it will return json containing a list of thing objects. You could write a client for this api like this:

```C#
//New up a new client
dynamic me = new RESTClient ();

//Set the url 
me.Url = "http://someapi.com";

//Get Things!
var things = (IEnumerable<Thing>)client.GetThings<IEnumerable<Thing>>();
```

ImpromptuInterface lets you do this:


```C#
//create an iterface to make it place nice again
public interface IAmAThingGetter
{
	IEnumerable<Thing> GetThings();
}

//makes client behave typesafe
IAmAThingGetter client = Impromptu.ActLike<IAmAThingGetter>(me);

//makes things typesafe-ish :)
var things = client.GetThings();

```

Isn't it cute. More to come see the tests for more usages. 

##Recently added##

* Automatic xml deserialisation to the generic function argument type if the contenttype is `application/xml`.
* Support for single noun function calls `client.Resolve("id");` will result in a GET /resolve/id
* Uri composition adjustable by Implementing `IUriComposer` and passing it to the RestClient's constructor
* Serialisation of .net json dates `/Date(millisecs-since-epoch)/` hacked into SimpleJson.

## To-do's ##

* Write more documentation
* Get feedback
* Content Negotiation + Deserialisation (protocol-buggers)
* XML->dynamic and JSON->dynamic should be shipped with DRC
* Glue things together with TinyIOC.
* Maybe loose the ImpromtuInterface ref
