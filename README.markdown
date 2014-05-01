# DynamicRestClient #

[![Build status](https://ci.appveyor.com/api/projects/status/foykr8ldk1jqegot)](https://ci.appveyor.com/project/albertjan/dynamicrestclient)

A simple REST client. Using Dynamics.

Lets asume there is a api at someapi.com which has a route `/things` to which you can do a get request it will return json containing a list of thing objects. You could write a client for this api like this:

```C#
//New up a new client
dynamic me = new RESTClient ();

//Set the url 
me.Url = "http://someapi.com";

//Get Things!
IEnumerable<Thing> things = client.GetThings();
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

* Automatic deserialisation of the response object to the type of the variable it's put into.
* Automatic xml deserialisation to the generic function argument type if the contenttype is `application/xml`.
* Support for single noun function calls `client.Resolve("id");` will result in a GET /resolve/id.
* Uri composition adjustable by Implementing `IUriComposer` and registering it with the container. 
* Serialisation of .net json dates `/Date(millisecs-since-epoch)/` hacked into SimpleJson.
* Glue things together with TinyIOC.

##To-do's##

* Write more documentation
* Get feedback
* Content Negotiation + Deserialisation (protocol-buggers)
* XML->dynamic and JSON->dynamic should be shipped with DRC
* Maybe loose the ImpromtuInterface ref (half-done lost it in mono. don't think I will it's a nice tool)
