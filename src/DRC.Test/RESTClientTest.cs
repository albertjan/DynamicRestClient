namespace DRC.Test
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using DRC;
    using DRC.Defaults;
    using DRC.Interfaces;
    using NUnit.Framework;

    [TestFixture]
    public class RESTClientTest
    {
        [SetUp]
        public void Init()
        {
            WebRequest.RegisterPrefix ("test", new TestWebRequestCreate ());
        }

        [Test]
        public void InputAndOutputExtensionsMethodTest()
        {
            var isOutputDelegate = new Func<string, byte[]>(s => Encoding.UTF8.GetBytes(s));
            var isInputDelegate = new Func<WebResponse, string> (s => "test");
            var inputLikeAction = new Action<string, byte[]>((s, b) => b.ToString());
            Assert.IsTrue(isInputDelegate.IsInput ());
            Assert.IsFalse(isInputDelegate.IsOutput ());
            Assert.IsTrue(isOutputDelegate.IsOutput ());
            Assert.IsFalse (isOutputDelegate.IsInput ());
            Assert.IsFalse(inputLikeAction.IsInput());
        }

        [Test]
        public void SkipLastNExtensionMethodTest()
        {
            var test = new[] {1, 2, 3, 4}.SkipLastN(1);
            Assert.AreEqual(new [] { 1, 2, 3 }, test);
            test = test.SkipLastN(2);
            Assert.AreEqual(new [] { 1 }, test);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureExceptionWhenGenericTypeDoesNotEqualInputEditorOutputType()
        {
            dynamic me = new RESTClient();
            me.GetTest.In = new Func<WebResponse, int>(s => 1);
            me.GetTest<string>("test");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureExceptionWhenThereAreArgumentsMissingForOutputEditor()
        {
            dynamic me = new RESTClient ();
            //me.GetTest.In = new Func<Stream, int> (s => 1);
            me.GetTest.Out =
                new Func<int, int, string, long, int, string, string, byte[]>((i1, i2, s1, l1, i3, s2, s3) => new byte[12]);
            me.GetTest (1, 2, "3", (long)4, /* missing5,*/ "6", "7");
        }

        [Test]
        public void EnsureThatTheSpecifiedInputEditorGetsUsed()
        {
            TestWebRequestCreate.CreateTestRequest ("dummy");
            dynamic me = new RESTClient ();
            me.Url = "test://test";
            me.GetTest.In = new Func<WebResponse, string>(w =>
            {
                string test;
                using (var sr = new StreamReader (w.GetResponseStream ()))
                    test = sr.ReadToEnd();

                Assert.AreEqual(test, "dummy");
                return "";
            });
            me.GetTest();
        }

        [Test]
        public void EnsureThatTheSpecifiedOuputEditorGetsUsed ()
        {
            TestWebRequestCreate.CreateTestRequest ("dummy");
            dynamic me = new RESTClient ();
            me.Url = "test://test";
            me.GetTest.Out = new Func<string, byte[]>(s =>
            {
                Assert.AreEqual(s, "test");
                return Encoding.UTF8.GetBytes(s);
            });
            me.GetTest ("test");
        }

        [Test]
        public void EnsureCorrenctFillingOfOutputEditorArguments()
        {
            TestWebRequestCreate.CreateTestRequest ("dummy");
            dynamic me = new RESTClient ();
            me.Url = "test://test";
            me.GetTest.Out =
                new Func<int, int, string, long, int, string, string, byte[]> ((i1, i2, s1, l1, i3, s2, s3) =>
                {
                    Assert.AreEqual(i1, 1);
                    Assert.AreEqual (i2, 2);
                    Assert.AreEqual (s1, "3");
                    Assert.AreEqual (l1, 4);
                    Assert.AreEqual (i3, 5);
                    Assert.AreEqual (s2, "6");
                    Assert.AreEqual (s3, "7");
                    return new byte[16];
                });
            me.GetTest (1, 2, "3", (long) 4, 5, "6", "7");
        }

        [Test]
        public void EnsureCorrenctFillingOfOutputEditorArgumentsWhenTheyArentCorrectlySorted ()
        {
            TestWebRequestCreate.CreateTestRequest ("dummy");
            dynamic me = new RESTClient ();
            me.Url = "test://test";
            me.GetTest.Out =
                new Func<int, int, string, long, int, string, string, byte[]> ((i1, i2, s1, l1, i3, s2, s3) =>
                {
                    Assert.AreEqual (i1, 1);
                    Assert.AreEqual (i2, 2);
                    Assert.AreEqual (s1, "3");
                    Assert.AreEqual (l1, 4);
                    Assert.AreEqual (i3, 5);
                    Assert.AreEqual (s2, "6");
                    Assert.AreEqual (s3, "7");
                    return new byte[16];
                });
            me.GetTest (1, 2, 5, "3", "6", "7", (long) 4);
        }

        [Test]
        public void EnsureArgumentDelegatesTakePrecedenceOverPredefinedDelegates()
        {
            TestWebRequestCreate.CreateTestRequest (5);
            dynamic me = new RESTClient ();
            me.Url = "test://test";
            me.GetTest.In = new Func<WebResponse, int>(s =>
            {
                Assert.Fail("Wrong delegate got called!");
                using (var sr = new BinaryReader (s.GetResponseStream()))
                    return sr.ReadInt32();
                
            });

            var result = me.GetTest (new Func<WebResponse, int>(s => 4));

            Assert.AreEqual(result, 4);
        }

        [Test]
        public void UrlParameterArguments()
        {
            TestWebRequestCreate.CreateTestRequest ("dummy");
         
            dynamic me = new RESTClient();
            me.Url = "test://base";
            me.GetTest.In = new Func<WebResponse, Stream>(r =>
                                                              {
                                                                  Assert.AreEqual(new Uri("test://base/Test/param"), r.ResponseUri);
                                                                  return r.GetResponseStream();
                                                              });
            me.GetTest("param");

            TestWebRequestCreate.CreateTestRequest ("dummy");

            me.GetTest.In = new Func<WebResponse, Stream> (r =>
            {
                Assert.AreEqual (new Uri ("test://base/Test/param1/param2"), r.ResponseUri);
                return r.GetResponseStream ();
            });
            me.GetTest ("param1", "param2");
        }

        [Test]
        public void OutputEditorInArgumentWithLotsOfArguments ()
        {
            TestWebRequestCreate.CreateTestRequest("dummy");

            dynamic me = new RESTClient();
            me.Url = "test://base";

            var result = me.GetTest(1, 2, 3, 4, 5, (long) 6, "7", "8", true,
                       new Func<bool, string, long, string, int, int, int, int, int, byte[]>(
                           (a, b, c, d, e, f, g, h, i) => { 
                               Assert.IsTrue (a);
                               Assert.AreEqual (b, "7");
                               Assert.AreEqual (c, 6);
                               Assert.AreEqual (d,"8");
                               Assert.AreEqual (e, 1);
                               Assert.AreEqual (f, 2);
                               Assert.AreEqual (g, 3);
                               Assert.AreEqual (h, 4);
                               Assert.AreEqual (i, 5);
                               return new byte[16]; 
                           }),
                       new Func<WebResponse,WebResponse> (r => r));
            
            Assert.AreEqual(result.ResponseUri, new Uri("test://base/Test"));
        }

        [Test]
        public void EnsureAFunctionCallWithEverythingAlsoWorks ()
        {
            TestWebRequestCreate.CreateTestRequest("dummy");

            dynamic me = new RESTClient();
            me.Url = "test://base";

            var result = me.GetTest<string>(/*QueryString*/ 
                                            new {page = "1", items = "50"},
                                            /*OutputEditorArguments*/
                                            1, 2, 3, 4, "5", "6",
                                            /*URLParam*/    
                                            "param",
                                            /*OutputEditor*/
                                            new Func<int, string, int, string, int, int, byte[]>(
                                                (i1, s1, i2, s2, i3, i4) =>
                                                {
                                                    Assert.AreEqual (i1, 1);
                                                    Assert.AreEqual (s1, "5");
                                                    Assert.AreEqual (i2, 2);
                                                    Assert.AreEqual (s2, "6");
                                                    Assert.AreEqual (i3, 3);
                                                    Assert.AreEqual (i4, 4);
                                                    return new byte[16];
                                                }),
                                            /*InputEditor*/
                                            new Func<WebResponse, string>(w =>
                                            {
                                                Assert.AreEqual(w.ResponseUri,
                                                                new Uri(
                                                                    "test://base/Test/param?page=1&items=50"));
                                                using (var sr = new StreamReader(w.GetResponseStream()))
                                                    return sr.ReadToEnd();
                                            }));
            Assert.AreEqual(result, "dummy");
        }

        [Test]
        public void PutVerbTest()
        {
            var request = TestWebRequestCreate.CreateTestRequest("");
            dynamic me = new RESTClient ();
            me.Url = "test://base";
            me.PutTest.Out = new Func<int, byte[]>(BitConverter.GetBytes);
            me.PutTest(1);
            Assert.AreEqual(BitConverter.ToInt32(((MemoryStream)request.GetRequestStream()).ToArray(),0),1);
        }
        
        [Test]
        public void MultipleNounTest()
        {
            var request = TestWebRequestCreate.CreateTestRequest ("");

            dynamic me = new RESTClient ();
            me.Url = "test://base";
            me.GetMultipleNounTest.In = new Func<WebResponse, string>(w =>
            {
                Assert.AreEqual(new Uri("test://base/Multiple/Noun/Test"), w.ResponseUri);
                return "yes";
            });
            me.GetMultipleNounsTest();
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "A Functionname must have atleast one token.")]
        public void DefaultVerbResolverTestEmptyFunctionName()
        {
            new DefaultVerbResolver(new DefaultStringTokenizer()).ResolveVerb("");
        }

        [Test]
        public void DefaultVerbResolverTest ()
        {
            var get = new DefaultVerbResolver (new DefaultStringTokenizer ()).ResolveVerb ("Get");
            var put = new DefaultVerbResolver (new DefaultStringTokenizer ()).ResolveVerb ("Put");
            var post = new DefaultVerbResolver (new DefaultStringTokenizer ()).ResolveVerb ("Post");
            var del = new DefaultVerbResolver (new DefaultStringTokenizer ()).ResolveVerb ("Delete");
            var bananas = new DefaultVerbResolver (new DefaultStringTokenizer ()).ResolveVerb ("BananasBananas");
            Assert.AreEqual (get, Verb.GET);
            Assert.AreEqual (put, Verb.PUT);
            Assert.AreEqual (post, Verb.POST);
            Assert.AreEqual (del, Verb.DELETE);
            Assert.AreEqual (bananas, Verb.GET);
        }

        [Test]
        public void ShouldExecutePipeLineEntries()
        {
            TestWebRequestCreate.CreateTestRequest ("dummy");
            dynamic me = new RESTClient ();
            me.Url = "test://test";

            var pipelinetest = "apeshit";
            var pipelinetest2 = "apeshit";

            me.InputPipeLine.Add (2.5, Tuple.Create ("pipelineitem", new Action<WebResponse> (resp =>{ pipelinetest = "yep"; })));
            me.OutputPipeLine.Add (0.002, Tuple.Create ("pipelineitem", new Action<WebRequest> (resp => { pipelinetest2 = "yep"; })));

            me.GetTest.In = new Func<WebResponse, string> (w =>
            {
                string test;
                using (var sr = new StreamReader (w.GetResponseStream ()))
                    test = sr.ReadToEnd ();

                Assert.AreEqual (test, "dummy");
                return "";
            });
            me.GetTest ();

            Assert.AreEqual ("yep", pipelinetest);
            Assert.AreEqual ("yep", pipelinetest2);
        }

        [Test]
        public void ShouldUseSpecifiedUrl()
        {
            TestWebRequestCreate.CreateTestRequest ("dummy");
            dynamic me = new RESTClient ();
            me.Url = "test://test";

            me.GetTest.Url = "bananas/overthere";

            me.OutputPipeLine.Add (0.1, Tuple.Create ("hi", new Action<WebRequest> (r => Assert.AreEqual ("test://test/bananas/overthere", r.RequestUri.ToString()))));

            me.GetTest();
        }
    }

    class TestWebRequestCreate : IWebRequestCreate
    {
        static WebRequest nextRequest;
        static object lockObject = new object ();

        static public WebRequest NextRequest
        {
            get { return nextRequest; }
            set
            {
                lock (lockObject)
                {
                    nextRequest = value;
                }
            }
        }

        /// <summary>See <see cref="IWebRequestCreate.Create"/>.</summary>
        public WebRequest Create (Uri uri)
        {
            ((TestWebRequest)nextRequest).SetUri(uri);
            return nextRequest;
        }

        /// <summary>Utility method for creating a TestWebRequest and setting
        /// it to be the next WebRequest to use.</summary>
        /// <param name="response">The response the TestWebRequest will return.</param>
        public static TestWebRequest CreateTestRequest (string response)
        {
            TestWebRequest request = new TestWebRequest (response);
            NextRequest = request;
            return request;
        }

        public static TestWebRequest CreateTestRequest (int response)
        {
            TestWebRequest request = new TestWebRequest (response);
            NextRequest = request;
            return request;
        }
    }

    class TestWebRequest : WebRequest
    {

        MemoryStream requestStream = new MemoryStream ();
        MemoryStream responseStream;

        public override string Method { get; set; }
        public override string ContentType { get; set; }
        public override long ContentLength { get; set; }

        public override Uri RequestUri
        {
            get
            {
                return TestRequestUri;
            }
        }

        protected Uri TestRequestUri { get; set; }

        public void SetUri(Uri uri)
        {
            TestRequestUri = uri;
        }

        /// <summary>Initializes a new instance of <see cref="TestWebRequest"/>
        /// with the response to return.</summary>
        public TestWebRequest (string response)
        {
            responseStream = new MemoryStream (System.Text.Encoding.UTF8.GetBytes (response));
        }

        /// <summary>Initializes a new instance of <see cref="TestWebRequest"/>
        /// with the response to return.</summary>
        public TestWebRequest (int response)
        {
            responseStream = new MemoryStream (BitConverter.GetBytes(response));
        }

        /// <summary>Returns the request contents as a string.</summary>
        public string ContentAsString ()
        {
            return System.Text.Encoding.UTF8.GetString (requestStream.ToArray ());
        }

        /// <summary>See <see cref="WebRequest.GetRequestStream"/>.</summary>
        public override Stream GetRequestStream ()
        {
            if (requestStream.CanWrite)
                return requestStream;
            
            return new MemoryStream (requestStream.GetBuffer ());
        }

        /// <summary>See <see cref="WebRequest.GetResponse"/>.</summary>
        public override WebResponse GetResponse ()
        {
            return new TestWebReponse (responseStream, RequestUri);
        }
    }

    class TestWebReponse : WebResponse
    {
        Stream responseStream;
        private Uri requestUri;

        public override Uri ResponseUri
        {
            get
            {
                return requestUri;
            }
        }

        /// <summary>Initializes a new instance of <see cref="TestWebReponse"/>
        /// with the response stream to return.</summary>
        public TestWebReponse (Stream responseStream)
        {
            this.responseStream = responseStream;
        }

        /// <summary>Initializes a new instance of <see cref="TestWebReponse"/>
        /// with the response stream to return.</summary>
        public TestWebReponse (Stream responseStream, Uri uri)
        {
            this.requestUri = uri;
            this.responseStream = responseStream;
        }

        /// <summary>See <see cref="WebResponse.GetResponseStream"/>.</summary>
        public override Stream GetResponseStream ()
        {
            return responseStream;
        }
    }
}
