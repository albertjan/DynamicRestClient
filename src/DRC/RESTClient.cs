namespace DRC
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    using Defaults;
    using Interfaces;

    using ImpromptuInterface;
    
    /// <summary>
    /// Set the URL first.
    /// 
    /// GetOrders() will result in GET URL + /orders will try to deserialize  to
    /// 
    /// GetOrders(
    ///     new {
    ///             page = 1,
    ///             items = 50
    ///         }); will result in GET URL + /orders?page=1&items=50
    /// 
    /// During the first call it will determine the deserialization method and type. This will be cached 
    /// while it lives. If it stops working it will try to redetermine the method and types. If it can't 
    /// find the type it will return an expandoObject containing properties coresponding to the name and 
    /// if known type specified in the message. In a perfect world :P it should.
    /// </summary>
    public class RESTClient: DynamicObject 
    {
        private readonly Dictionary<string, List<Delegate>> _editorDelegates = new Dictionary<string, List<Delegate>> ();
        
        public INounResolver NounResolver { get; set; }
        public IQueryStringResolver QueryStringResolver { get; set; }
        public IVerbResolver VerbResolver { get; set; }
        public IStringTokenizer StringTokenizer { get; set; }
        public IUriComposer UriComposer { get; set; }
        public string Url { get; set; }



        public RESTClient(INounResolver nounResolver = null, 
            IQueryStringResolver queryStringResolver = null,
            IVerbResolver verbResolver = null,
            IStringTokenizer stringTokenizer = null,
            IUriComposer uriComposer = null)
        {
            StringTokenizer = stringTokenizer ?? new DefaultCachedStringTokenizer ();
            NounResolver = nounResolver ?? new DefaultNounResolver (StringTokenizer);
            QueryStringResolver = queryStringResolver ?? new DefaultQueryStringResolver(StringTokenizer);
            VerbResolver = verbResolver ?? new DefaultVerbResolver(StringTokenizer);
            UriComposer = uriComposer ?? new DefaultUriComposer();

            //create input pipeline and store response
            InputPipeLine = new Dictionary<double, Tuple<string, Action<WebResponse>>>
            {{9999, new Tuple<string, Action<WebResponse>>("storeresponse", response => WebResponse = response)}};
            OutputPipeLine = new Dictionary<double, Tuple<string, Action<WebRequest>>>();

            ClientCertificateParameters = null;
        }

        public ClientCertificateParameters ClientCertificateParameters { get; set; }

        public Dictionary<double, Tuple<string, Action<WebResponse>>> InputPipeLine { get; set; }
        public Dictionary<double, Tuple<string, Action<WebRequest>>> OutputPipeLine { get; set; }

        /// <summary>
        /// Get an InputOutputEditorSetters object for _every_ name you put in here.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember (GetMemberBinder binder, out object result)
        {
            result = new InputOutputEditorSetters (binder.Name, _editorDelegates, NounResolver);
            return true;
        }

        /// <summary>m
        /// Entry function for everything you can come up with.!
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result)
        {
            //get the Type argument from the binder
            var typeArg = binder.ActLike<ITypeArguments>().m_typeArguments.FirstOrDefault();

            //Mangle the function name into nouns and verbs
            var noun = NounResolver.ResolveNoun (binder.Name);
            var verb = VerbResolver.ResolveVerb (binder.Name);
            
            var doCallParameters = GetDoCallParameters(typeArg, binder.Name, args);
            
            //dynamicly call a generic function
            var method = GetType().GetMethod("DoCall").MakeGenericMethod(doCallParameters.GenericTypeArgument);
            result = method.Invoke (this, new object[] { verb, noun, doCallParameters.QueryDict, doCallParameters.InputEditor, doCallParameters.UrlParameters, doCallParameters.Payload });

            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(Stream))
            {
                result = WebResponse.GetResponseStream();
                return result != null || base.TryConvert(binder, out result);
            }

            //var func = typeof(Func<,>).MakeGenericType(typeof(WebResponse), binder.Type);
            var me = typeof(RESTClient).GetMethod("GetDeserializationMethod", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(binder.Type);

            var trie = me.Invoke(this, new object[] {WebResponse});
            
            result = trie;
            return result != null || base.TryConvert(binder, out result);
        }

        /// <summary>
        /// Find an input editor from the function arguments.
        /// </summary>
        /// <param name="functionArguments"></param>
        /// <returns></returns>
        private Delegate FindInputEditor(IEnumerable<object> functionArguments)
        {
            return functionArguments.Where (o => o.GetType ().Name == "Func`2").Cast<Delegate> ().FirstOrDefault (d => d.IsInput ());
        }


        /// <summary>
        /// Find an output editor from the function arguments.
        /// </summary>
        /// <param name="functionArguments"></param>
        /// <returns></returns>
        private Delegate FindOutputEditor (IEnumerable<object> functionArguments)
        {
            return functionArguments.Where (o => o.GetType ().Name.Contains("Func")).Cast<Delegate> ().FirstOrDefault (d => d.IsOutput ());
        }

        /// <summary>
        /// Here's where the magic happens. It collects the information to do the request and pull it through the correct delgates. 
        /// </summary>
        /// <param name="typeArg"></param>
        /// <param name="binderName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private CallMethodAgruments GetDoCallParameters(Type typeArg, string binderName, IEnumerable<object> args)
        {
            var argList = args.ToList ();
            
            //find the input and output editors if specified. or use the defaults.
            var retval = new CallMethodAgruments
            {
                InputEditor = (_editorDelegates.ContainsKey(binderName) &&
                            _editorDelegates[binderName].FirstOrDefault(d => d.IsInput()) != null)
                                ? _editorDelegates[binderName].First(d => d.IsInput())
                                : new Func<WebResponse, RESTClient>(s => (dynamic)this),
                OutputEditor = (_editorDelegates.ContainsKey(binderName) &&
                                _editorDelegates[binderName].FirstOrDefault(d => d.IsOutput()) != null)
                                ? _editorDelegates[binderName].First(d => d.IsOutput())
                                : new Func<byte[], ClientRequest>(b => new ClientRequest{Body = b})
            };

            //if the arguments have and input or output editor use that one.
            retval.InputEditor = FindInputEditor (argList) ?? retval.InputEditor;
            retval.OutputEditor = FindOutputEditor (argList) ?? retval.OutputEditor;
            
            retval.GenericTypeArgument = typeArg ?? retval.InputEditor.GetType().GetGenericArguments().Last();

            if (typeArg != null && retval.InputEditor.GetType ().GetGenericArguments ().Last () != typeArg && retval.InputEditor.GetType ().GetGenericArguments ().Last () == typeof(RESTClient))
            {

                var func = typeof (Func<,>).MakeGenericType(typeof(WebResponse), typeArg);
                var me = typeof (RESTClient).GetMethod ("GetDeserializationMethod", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod (typeArg);

                retval.InputEditor = Delegate.CreateDelegate(func, this, me, true);
                     
                //throw new ArgumentException ("GenericArgument not the same as TOut of the InputEditor");
            }

            if (typeArg != null && retval.InputEditor.GetType().GetGenericArguments().Last() != typeArg)
                throw new ArgumentException("GenericArgument not the same as TOut of the InputEditor");


            //find the anonymous type to turn into the querystring
            var anonymousQueryObject = argList.FirstOrDefault (o => o.GetType ().Name.Contains ("Anonymous"));
            IEnumerable<KeyValuePair<string, string>> queryDict = null;
            if (anonymousQueryObject != null)
                queryDict = QueryStringResolver.ResolveQueryDict(binderName, anonymousQueryObject);
            
            retval.QueryDict = queryDict;

            //outputeditorargumentselection and payload generation empty request
            if (retval.OutputEditor.GetType().GetGenericArguments().First() == typeof(byte[]))
                if (argList.FirstOrDefault(o => o is byte[]) == null)
                    argList.Add(new byte[0]);

            //get the types from the outputeditor delegate arguments haal except the last which is the output type (byte[])
            var outputEditorArgumentsTypes = retval.OutputEditor.GetType().GetGenericArguments().SkipLastN(1);
            var sortedArgs = new List<object> ();

            //Call the outputeditor delegate with the arguments given to the function. And invoke it.
            try
            {
                var typeOrderDict = new Dictionary<string, int>();
                foreach (var outputEditorArgumentsType in outputEditorArgumentsTypes.Select(t=>t.Name))
                {
                    if (!typeOrderDict.ContainsKey(outputEditorArgumentsType))   
                        typeOrderDict.Add(outputEditorArgumentsType, 1);
                    else
                        typeOrderDict[outputEditorArgumentsType]++;

                    string type = outputEditorArgumentsType;

                    sortedArgs.Add(argList.TakeNthOccurence(o=>o.GetType().Name == type, typeOrderDict[outputEditorArgumentsType]));
                }

                retval.Payload = (ClientRequest)retval.OutputEditor.FastDynamicInvoke (sortedArgs.ToArray ()); /* FastDynamicInvoke wants an
                                                                                                         * array */
            }
            catch (Exception ex)
            {
                throw new ArgumentException ("Could not find all arguments to construct output message! (Expected: " +
                                     retval.OutputEditor.GetType ().GetGenericArguments ().SkipLastN (1).Select (t => t.Name).Aggregate ((t1, t2) => t1 + ", " + t2)
                                     + ") (Found: " + (argList.Count == 0 ? "none" : argList.Select (a => a.GetType ().Name).Aggregate ((t1, t2) => t1 + ", " + t2)) + ")", ex);
            }


            //find url parameters
            retval.UrlParameters = argList.Where (o => !o.GetType ().Name.Contains ("Func`")) //no funcs
                                          .Where (o => o != argList.FirstOrDefault(ob => ob.GetType().Name.Contains("Anonymous"))) //not the first anonymous object
                                          .Where (o => !sortedArgs.Contains (o)) //no output editor arguments
                                          .ToArray ();
            return retval;
        }

        protected WebResponse WebResponse { get; set; }

// ReSharper disable UnusedMember.Local
        private T GetDeserializationMethod<T>(WebResponse ofT)
        {
            if (ofT.ContentType == "application/json")
            {
                using (var sr = new StreamReader (ofT.GetResponseStream ()))
                {
                    return SimpleJson.DeserializeObject<T> (sr.ReadToEnd ());
                }    
            }

            if (ofT.ContentType == "application/xml" || ofT.ContentType.StartsWith("text/xml"))
            {
                return (T) new XmlSerializer (typeof (T)).Deserialize (ofT.GetResponseStream ());
            } 

            //if (typeof(T) == typeof(string))
            //{
            //    using (var sr = new StreamReader(ofT.GetResponseStream()))
            //        return (T) sr.ReadToEnd();
            //}
            
            throw new Exception("Can't Deserialise (" + ofT.ContentType + ")");
        }
// ReSharper restore UnusedMember.Local

        public T DoCall<T> (Verb callMethod, string site, IEnumerable<KeyValuePair<string, string>> queryString, Func<WebResponse, T> editor, object[] urlParameters = null, ClientRequest what = null)
        {
            var wr = WebRequest.Create (UriComposer.ComposeUri (Url, site, urlParameters, queryString));
            wr.Method = callMethod.ToString();
            
            if (ClientCertificateParameters != null)
            {
                var store = new X509Store(ClientCertificateParameters.StoreName, ClientCertificateParameters.StoreLocation);
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(ClientCertificateParameters.FindBy, ClientCertificateParameters.FindString, false);
                if (certs.Count == 0) throw new CryptographicException("No certificates found.");
                ((HttpWebRequest) wr).ClientCertificates.AddRange(certs);
            }

            if (what != null && (what.Headers != null))
            {
                foreach (var header in what.Headers)
                {
                    switch (header.Key)
                    {
                        case "Connection":
                        case "Content-Length":
                            continue;
                        case "Expect":
                            ((HttpWebRequest)wr).Expect = header.Value;
                            continue;
                        case "Date":
                            //((HttpWebRequest)wr).Date = header.Value;
                            //should do some parsing here.
                            continue;
                        case "If-Modified-Since":
                            //((HttpWebRequest)wr).IfModifiedSince = header.Value;
                            //again with the parsing
                            continue;
                        case "Range":
                            //((HttpWebRequest)wr).AddRange() = header.Value;
                            //needs more impl.
                            continue;
                        case "Referer":
                            ((HttpWebRequest)wr).Referer = header.Value;
                            continue;
                        case "Transfer-Encoding":
                            if (String.Equals(header.Value, "chucked", StringComparison.OrdinalIgnoreCase))
                                ((HttpWebRequest)wr).SendChunked = true;
                            
                            //ignore others.
                            //((HttpWebRequest)wr).TransferEncoding = header.Value;
                            continue;
                        case "Host":
                            ((HttpWebRequest)wr).Host = header.Value;
                            continue;
                        case "User-Agent":
                            ((HttpWebRequest) wr).UserAgent = header.Value;
                            continue;
                        case "Content-Type":
                            wr.ContentType = header.Value;
                            continue;
                        case "Accept":
                            ((HttpWebRequest) wr).Accept = header.Value;
                            continue;
                        default:
                            wr.Headers.Add (header.Key, header.Value);
                            break;
                    }
                }
                if (!String.IsNullOrWhiteSpace(what.ContentType)) wr.ContentType = what.ContentType;
            }

            //call output pipelinebefore writing body (httpwebrequest will send the body immediatly
            foreach (var pipelineItem in OutputPipeLine.OrderBy (p => p.Key))
            {
                pipelineItem.Value.Item2 (wr);
            }

            switch (callMethod)
            {
                case Verb.GET:
                    //no get Body because: http://tech.groups.yahoo.com/group/rest-discuss/message/9962
                case Verb.DELETE:
                    //no delete Body because: http://stackoverflow.com/questions/299628/is-an-entity-body-allowed-for-an-http-delete-request
                    //esp: "The DELETE method requests that the origin server delete the resource _identified_ by the Request-URI."
                    break;
                case Verb.PUT:
                case Verb.POST:
                    if (what != null)
                    {
                        wr.ContentLength = what.Body.Length;
                        if (what.Body.Length > 0)
                            using (var sr = new BinaryWriter(wr.GetRequestStream()))
                            {
                                sr.Write(what.Body);
                                sr.Flush();
                            }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("callMethod");
            }
            WebResponse resp = null;

            //swallow stupid exceptions!!
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                resp = wr.GetResponse();
            }catch(WebException e)
            {
                resp = e.Response;
            }
            // ReSharper restore EmptyGeneralCatchClause

            foreach (var pipelineItem in InputPipeLine.OrderBy(p=> p.Key))
            {
                pipelineItem.Value.Item2(resp);
            }

            return editor (resp);
        }
    }

    public class ClientCertificateParameters
    {
        public string FindString { get; set; }
        public X509FindType FindBy { get; set; }
        public StoreLocation StoreLocation { get; set; }
        public StoreName StoreName { get; set; }
    }

    public class InputOutputEditorSetters
    {
        public string BinderName { get; set; }
        public Dictionary<string, List<Delegate>> EditorDelegates { get; set; }
        public List<Delegate> CurrentEditorDelegates { get; set; }
        public INounResolver NounResolver { get; set; }

        public InputOutputEditorSetters (string binderName, Dictionary<string, List<Delegate>> editorDelegates, INounResolver nounResolver)
        {
            if (!editorDelegates.ContainsKey (binderName))
                editorDelegates.Add (binderName, new List<Delegate> ());

            BinderName = binderName;
            EditorDelegates = editorDelegates;
            NounResolver = nounResolver;
            CurrentEditorDelegates = EditorDelegates[BinderName];
        }

        public Delegate In
        {
            get
            {
                return CurrentEditorDelegates.FirstOrDefault (d => d.IsInput ());
            }
            set
            {
                if (In != null)
                    CurrentEditorDelegates.Remove (In);
                CurrentEditorDelegates.Add (value);
            }
        }

        public Delegate Out
        {
            get
            {
                return CurrentEditorDelegates.FirstOrDefault (d => d.IsOutput ());
            }
            set
            {
                if (Out != null)
                    CurrentEditorDelegates.Remove (Out);
                CurrentEditorDelegates.Add (value); 
            }
        }

        public string Url
        {
            get { return NounResolver.PredefinedUrls[BinderName]; }
            set
            {
                if (NounResolver.PredefinedUrls.ContainsKey (BinderName))
                    NounResolver.PredefinedUrls[BinderName] = value;
                else
                    NounResolver.PredefinedUrls.Add (BinderName, value);
            } 
        }
    }

    public class CallMethodAgruments
    {
        public Type GenericTypeArgument { get; set; }
        public Delegate InputEditor { get; set; }
        public Delegate OutputEditor { get; set;}
        public IEnumerable<KeyValuePair<string,string>> QueryDict { get; set; }
        public object[] UrlParameters { get; set; }
        public ClientRequest Payload { get; set; }
    }

    public interface ITypeArguments
    {
        IList<Type> m_typeArguments { get; }
    }

    public class ClientRequest
    {
        public byte[] Body { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string,string> Headers { get; set; }
    }

    public static class Extensions
    {
        public static T TakeNthOccurence<T> (this IEnumerable<T> source, Func<T, bool> predicate, int n)
        {
            var it = source.GetEnumerator();
            var count = 0;
            while (it.MoveNext())
            {
                if (predicate (it.Current)) 
                    count++;
                 if (count == n) return it.Current;
            }
            throw new InvalidOperationException ("No element satisfies the condition in predicate. ");

        }

        public static bool IsInput (this Delegate inputDelegate)
        {
            return (inputDelegate.GetType ().Name.Contains ("Func") &&
                    inputDelegate.GetType ().GetGenericArguments ().First () == typeof (WebResponse));
        }

        public static bool IsOutput (this Delegate outputDelegate)
        {
            return (outputDelegate.GetType ().Name.Contains ("Func") &&
                    outputDelegate.GetType ().GetGenericArguments ().Last () == typeof (ClientRequest));
        }

        public static IEnumerable<T> SkipLastN<T> (this IEnumerable<T> source, int n)
        {
            var it = source.GetEnumerator ();
            bool hasRemainingItems;
            var cache = new Queue<T> (n + 1);
            do
            {
                if (hasRemainingItems = it.MoveNext ())
                {
                    cache.Enqueue (it.Current);
                    if (cache.Count > n)
                        yield return cache.Dequeue ();
                }
            } while (hasRemainingItems);
        }

    }
}