using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using D2L.Extensibility.AuthSdk.IntegrationTests.Domain;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;

using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {
	[TestFixture]
	public class IntegrationTests {
		[TestFixtureSetUp]
		public void ReadConfig() {
			m_appId = ConfigurationManager.AppSettings[ "appId" ];
			m_appKey = ConfigurationManager.AppSettings[ "appKey" ];
			m_userId = ConfigurationManager.AppSettings[ "userId" ];
			m_userKey = ConfigurationManager.AppSettings[ "userKey" ];
			m_scheme = ConfigurationManager.AppSettings[ "scheme" ];
			m_host = ConfigurationManager.AppSettings[ "host" ];
			m_port = Int32.Parse( ConfigurationManager.AppSettings[ "port" ] );
		}

		[SetUp]
		public void SetUpUserContext() {
			var factory = new D2LAppContextFactory();
			m_appContext = factory.Create( m_appId, m_appKey );
			var apiHost = GetDefaultApiHost();
			m_userContext = m_appContext.CreateUserContext( m_userId, m_userKey, apiHost );
			m_anonContext = m_appContext.CreateAnonymousUserContext( apiHost );
			m_badUserContext = m_appContext.CreateUserContext( "foo", "bar", apiHost );

		}

		[TearDown]
		public void Sleep() {
			//Tried on valence: 5 seconds, 7/10 tests succeed; 10 and 20 seconds, 9/10.
			//Failures are due to timeouts.
			//Thread.Sleep( 20*1000 );
		}

		[Test]
		public void LearningTest_SendRequestWithBadKeys_ThrowsWebException() {
			var request = PrepareApiRequest( m_badUserContext, GET_ORGANIZATION_INFO_ROUTE );

			Assert.Throws<WebException>( () => request.GetResponse() );
		}

		[Test]
		public void LearningTest_SendRequestWithBadKeys_ResponseBodyContains_Invalid_token() {
			var request = PrepareApiRequest( m_badUserContext, GET_ORGANIZATION_INFO_ROUTE );

			try {
				request.GetResponse();
			} catch( WebException ex ) {
				var responseBody = ReadResponseContents( ex.Response as HttpWebResponse );
				Assert.IsTrue( responseBody.Equals( "Invalid token", StringComparison.InvariantCulture ) );
			}
		}

        [Test]
		public void SendRequestWithBadKeys_ResponseInterpretationIs_InvalidSig() {
			var request = PrepareApiRequest( m_badUserContext, GET_ORGANIZATION_INFO_ROUTE );

			try {
				request.GetResponse();
			} catch( WebException ex ) {
				var exceptionWrapper = new D2LWebException( ex );
				var interpretation = m_userContext.InterpretResult( exceptionWrapper );
				Assert.AreEqual( RequestResult.RESULT_INVALID_SIG, interpretation );
			}
		}

		[Test]
		public void SendRequest_WhenBadHostSpec_UnhandledException() {
			var badApiHost = new HostSpec( ChangeScheme( m_scheme ), m_host, m_port );
			var badAnonContext = m_appContext.CreateAnonymousUserContext( badApiHost );
			var request = PrepareApiRequest( badAnonContext, GET_VERSIONS_ROUTE );

			Assert.Throws<WebException>( () => request.GetResponse() );
		}

		private string ChangeScheme( string scheme ) {
			return scheme == "https" ? "http" : "https";
		}

		[Test]
		public void SendRequest_AtUrlForAuthentication_ResponseReceived() {
			var hostSpec = new HostSpec( m_scheme, m_host, m_port );
			var landingUrl = new UriBuilder( m_scheme, m_host, m_port, GET_ORGANIZATION_INFO_ROUTE ).Uri;
			var uri = m_appContext.CreateUrlForAuthentication( hostSpec, landingUrl );

			var request = CreateRequest( uri );

			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendRequest_AtUrlForAuthentication_StatusCodeIs200() {
			var hostSpec = new HostSpec( m_scheme, m_host, m_port );
			var landingUrl = new UriBuilder( m_scheme, m_host, m_port, GET_ORGANIZATION_INFO_ROUTE ).Uri;
			var uri = m_appContext.CreateUrlForAuthentication( hostSpec, landingUrl );

			var request = CreateRequest( uri );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
			}
		}
		
		[Test]
		public void SendAuthenticatedRequest_ResponseReceived() {
			var request = PrepareApiRequest( m_userContext, GET_ORGANIZATION_INFO_ROUTE );

			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendAuthenticatedRequest_StatusCodeIs200() {
			var request = PrepareApiRequest( m_userContext, GET_ORGANIZATION_INFO_ROUTE );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
			}
			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendAuthenticatedRequest_ResponseContentsIsNotEmpty() {
			var request = PrepareApiRequest( m_userContext, GET_ORGANIZATION_INFO_ROUTE );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				string contents = ReadResponseContents( response );
				Assert.IsNotNullOrEmpty( contents );
			}
		}
		
		[Test]
		public void SendAuthenticatedRequest_ResponseContents_CanBeDeserializedAsJson() {
			var request = PrepareApiRequest( m_userContext, GET_ORGANIZATION_INFO_ROUTE );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				var org = DeserializeResponseContents<Organization>( response );
				Assert.IsNotNull( org );
			}
		}

		[Test]
		public void SendAnonymousRequest_ResponseReceived() {
			var request = PrepareApiRequest( m_anonContext, GET_VERSIONS_ROUTE);

			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendAnonymousRequest_StatusCodeIs200() {
			var request = PrepareApiRequest( m_anonContext, GET_VERSIONS_ROUTE );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
			}
			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendAnonymousRequest_ResponseContentsIsNotEmpty() {
			var request = PrepareApiRequest( m_anonContext, GET_VERSIONS_ROUTE );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				string contents = ReadResponseContents( response );
				Assert.IsNotNullOrEmpty( contents );
			}
		}

		[Test]
		public void SendAnonymousRequest_ResponseContents_CanBeDeserializedAsJson() {
			var request = PrepareApiRequest( m_anonContext, GET_VERSIONS_ROUTE );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				var versions = DeserializeResponseContents<ProductVersions[]>( response );
				Assert.IsNotNull( versions );
			}
		}
		
		[Test]
		public void SendAuthenticatedRequestWithDelayedTimestamp_ResponseMustContainTimeOffset() {

			m_appContext = CreateAppContextWithDelay( m_appId, m_appKey, TEST_TIME_DELAY );

			var userContext = CreateUserOperationContext();
			var uri = userContext.CreateAuthenticatedUri( GET_ORGANIZATION_INFO_ROUTE, "GET" );

			var request = (HttpWebRequest) WebRequest.Create( uri );
			request.Method = "GET";

			try {
				request.GetResponse();
			} catch( WebException ex ) {
				string responseContents = ReadResponseContents( ex.Response as HttpWebResponse );

				Assert.That( responseContents, Is.StringMatching( "Timestamp out of range\\s?(\\d+)" )  );
				return;
			}
			Assert.Fail( "Expected WebException was not thrown");
		}

		[Test]
		public void RetryAuthenticatedRequestWithCorrectedTimestamp_ResultCodeIs200() {

			m_appContext = CreateAppContextWithDelay( m_appId, m_appKey, TEST_TIME_DELAY );

			var userContext = CreateUserOperationContext();
			var uri = userContext.CreateAuthenticatedUri( GET_ORGANIZATION_INFO_ROUTE, "GET" );
			var request = (HttpWebRequest) WebRequest.Create( uri );
			request.Method = "GET";
			try {
				request.GetResponse();
			}
			catch( WebException ex ) {
				var exWrapper = new D2LWebException( ex );
				userContext.InterpretResult( exWrapper );
			}
			var retryUri = userContext.CreateAuthenticatedUri( GET_ORGANIZATION_INFO_ROUTE, "GET" );
			m_asyncRequest = (HttpWebRequest) WebRequest.Create( retryUri );
			m_asyncRequest.Method = "GET";

			m_getResponseAsyncCallbackHandle = new EventWaitHandle( false, EventResetMode.AutoReset );
			m_asyncRequest.BeginGetResponse( GetResponseAsyncCallback, null );
			m_getResponseAsyncCallbackHandle.WaitOne( 60 * 1000 );
			Assert.AreEqual( HttpStatusCode.OK, m_asyncResponseStatusCode );
		}

		private HttpWebRequest m_asyncRequest;
		private HttpStatusCode m_asyncResponseStatusCode;
		private EventWaitHandle m_getResponseAsyncCallbackHandle;

		private void GetResponseAsyncCallback( IAsyncResult ar ) {
			var response = m_asyncRequest.EndGetResponse( ar ) as HttpWebResponse;
			if( response != null ) {
				m_asyncResponseStatusCode = response.StatusCode;
			}
			m_getResponseAsyncCallbackHandle.Set();
		}


		private HttpWebResponse RetryGetRequest( ID2LUserContext opContext, string route ) {
			var mainRequest = CreateGetRequestForRoute( opContext, route );
			try {
				return mainRequest.GetResponse() as HttpWebResponse;
			} catch( WebException ex ) {
				var exWrapper = new D2LWebException( ex );
				opContext.InterpretResult( exWrapper );
			}
			var retryRequest = CreateGetRequestForRoute( opContext, route );
			return retryRequest.GetResponse() as HttpWebResponse;
		}

		private static HttpWebRequest CreateGetRequestForRoute(
			ID2LUserContext opContext, string route ) {

			var mainUri = opContext.CreateAuthenticatedUri( route, "GET" );
			var request = (HttpWebRequest) WebRequest.Create( mainUri );
			request.Method = "GET";
			return request;
		}

		private HttpWebRequest PrepareApiRequest( ID2LUserContext userContext, string route ) {
			Uri apiUri = userContext.CreateAuthenticatedUri( route, "GET" );
			return CreateRequest( apiUri );
		}

		private static HttpWebRequest CreateRequest( Uri uri ) {
			var request = (HttpWebRequest) WebRequest.Create( uri );
			request.Method = "GET";
			request.Accept = "*/*";

			//	keep the timeout at least 10 seconds,
			//	otherwise some important errors turn into "operation timed-out" errors
			request.Timeout = 10*1000;
			return request;
		}

		private string ReadResponseContents( HttpWebResponse response ) {
			using( var stream = response.GetResponseStream() ) {
				using( var reader = new StreamReader( stream, Encoding.UTF8 ) ) {
					return reader.ReadToEnd();
				}
			}
		}

		private T DeserializeResponseContents<T>( HttpWebResponse response ) where T : class {
			string contents = ReadResponseContents( response );
			var serializer = new JavaScriptSerializer();
			var resource = serializer.Deserialize<T>( contents );
			return resource;
		}

		private static ID2LAppContext CreateAppContextWithDelay( string appId, string appKey, long delaySeconds ) {

			var timestampProvider = CreateTimestampProviderDelay( delaySeconds );
			var factory = new D2LAppContextFactory( timestampProvider );
			return factory.Create( appId, appKey );
		}

		private static ITimestampProvider CreateTimestampProviderDelay( long seconds ) {
			return new LateTimestampProvider( seconds );
		}

		private ID2LUserContext CreateUserOperationContext() {
			var apiHost = new HostSpec( m_scheme, m_host, m_port );
			var opContext = m_appContext.CreateUserContext( m_userId, m_userKey, apiHost );
			return opContext;
		}

		private HostSpec GetDefaultApiHost() {
			return new HostSpec( m_scheme, m_host, m_port );
		}

		private ID2LAppContext m_appContext;
		private ID2LUserContext m_userContext;
		private ID2LUserContext m_anonContext;
		private ID2LUserContext m_badUserContext;
		private string m_appId;
		private string m_appKey;
		private string m_scheme;
		private string m_host;
		private int m_port;
		private string m_userId;
		private string m_userKey;

		private const string GET_ORGANIZATION_INFO_ROUTE = "/d2l/api/lp/1.0/organization/info";
		private const string GET_VERSIONS_ROUTE = "/d2l/api/versions/";
		private const long TEST_TIME_DELAY = 600L;
	}
}
