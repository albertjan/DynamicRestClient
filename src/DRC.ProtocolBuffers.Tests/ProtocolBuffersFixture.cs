namespace DRC.ProtocolBuffers.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    
    using NUnit.Framework;
    using ProtoBuf;
    //using DRC.ProtocolBuffers;
    
    [TestFixture]
    public class ProtocolBuffersFixture
    {
        [SetUp]
        public void Init()
        {
            WebRequest.RegisterPrefix("test", new TestWebRequestCreate());

            new ProtocolBuffersBodySerializer();
        }

        [Test]
        public void ShouldBeAbleToDeserializeProtocolBuffers()
        {
            byte[] buffer;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, new AModel { D = 0.1, I = 1, L = 2, S = "s" });
                buffer = ms.ToArray();
            }
            var test = TestWebRequestCreate.CreateTestRequest(buffer);
            test.ContentType = "application/protocol-buffers";
            dynamic me = new RESTClient();
            me.Url = "test://test";
            var a = me.GetTest<AModel>();
            Assert.AreEqual(typeof(AModel), a.GetType());
            Assert.AreEqual(a.D, 0.1);
            Assert.AreEqual(a.I, 1);
            Assert.AreEqual(a.L, 2L);
            Assert.AreEqual(a.S, "s");
        }
    }

    [ProtoContract]
    public class AModel
    {
        [ProtoMember(1)]
        public double D { get; set; }

        [ProtoMember(2)]
        public int I { get; set; }

        [ProtoMember(3)]
        public long L { get; set; }

        [ProtoMember(4)]
        public string S { get; set; }
    }

    class TestWebRequestCreate : IWebRequestCreate
    {
        static WebRequest nextRequest;
        static object lockObject = new object();

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
        public WebRequest Create(Uri uri)
        {
            ((TestWebRequest)nextRequest).SetUri(uri);
            return nextRequest;
        }

        /// <summary>Utility method for creating a TestWebRequest and setting
        /// it to be the next WebRequest to use.</summary>
        /// <param name="response">The response the TestWebRequest will return.</param>
        public static TestWebRequest CreateTestRequest(string response)
        {
            TestWebRequest request = new TestWebRequest(response);
            NextRequest = request;
            return request;
        }

        public static TestWebRequest CreateTestRequest(int response)
        {
            TestWebRequest request = new TestWebRequest(response);
            NextRequest = request;
            return request;
        }

        public static TestWebRequest CreateTestRequest(byte[] response)
        {
            TestWebRequest request = new TestWebRequest(response);
            NextRequest = request;
            return request;
        }
    }

    class TestWebRequest : WebRequest
    {

        MemoryStream requestStream = new MemoryStream();
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
        public TestWebRequest(string response)
        {
            responseStream = new MemoryStream(Encoding.UTF8.GetBytes(response));
        }

        /// <summary>Initializes a new instance of <see cref="TestWebRequest"/>
        /// with the response to return.</summary>
        public TestWebRequest(int response)
        {
            responseStream = new MemoryStream(BitConverter.GetBytes(response));
        }

        public TestWebRequest(byte[] response)
        {
            responseStream = new MemoryStream(response);
        }

        /// <summary>Returns the request contents as a string.</summary>
        public string ContentAsString()
        {
            return Encoding.UTF8.GetString(requestStream.ToArray());
        }

        /// <summary>See <see cref="WebRequest.GetRequestStream"/>.</summary>
        public override Stream GetRequestStream()
        {
            if (requestStream.CanWrite)
                return requestStream;

            return new MemoryStream(requestStream.GetBuffer());
        }

        /// <summary>See <see cref="WebRequest.GetResponse"/>.</summary>
        public override WebResponse GetResponse()
        {
            return new TestWebReponse(responseStream, RequestUri, ContentType);
        }
    }

    class TestWebReponse : WebResponse
    {
        Stream responseStream;
        private Uri requestUri;
        private string _contentType;

        public override Uri ResponseUri
        {
            get
            {
                return requestUri;
            }
        }

        public override WebHeaderCollection Headers
        {
            get
            {
                return new WebHeaderCollection();
            }
        }

        public override long ContentLength
        {
            get { return responseStream.Length; }
            set
            {
                //base.ContentLength = value;
            }
        }
        public override string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        /// <summary>Initializes a new instance of <see cref="TestWebReponse"/>
        /// with the response stream to return.</summary>
        public TestWebReponse(Stream responseStream)
        {
            this.responseStream = responseStream;
        }

        /// <summary>Initializes a new instance of <see cref="TestWebReponse"/>
        /// with the response stream to return.</summary>
        public TestWebReponse(Stream responseStream, Uri uri, string contentType = null)
        {
            requestUri = uri;
            _contentType = contentType;
            this.responseStream = responseStream;
        }

        /// <summary>See <see cref="WebResponse.GetResponseStream"/>.</summary>
        public override Stream GetResponseStream()
        {
            return responseStream;
        }
    }
}
