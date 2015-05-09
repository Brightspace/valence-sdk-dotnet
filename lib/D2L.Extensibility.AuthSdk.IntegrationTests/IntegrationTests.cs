using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using D2L.Extensibility.AuthSdk.IntegrationTests.Domain;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;
using D2L.Extensibility.AuthSdk.IntegrationTests.Providers;

using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {

	[TestFixture]
	public class IntegrationTests {

		[TestFixtureSetUp]
        public void FixtureSetup() {
            ReadConfig();
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
		public void SendRequest_AtUrlForAuthentication_ResponseReceived() {
			var hostSpec = new HostSpec( m_scheme, m_host, m_port );
            var landingUrl = new UriBuilder( m_scheme, m_host, m_port, RouteProvider.OrganizationInfoRoute ).Uri;
			var uri = m_appContext.CreateUrlForAuthentication( hostSpec, landingUrl );

            var request = RequestProvider.CreateRequest( uri );

			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendRequest_AtUrlForAuthentication_StatusCodeIs200() {
			var hostSpec = new HostSpec( m_scheme, m_host, m_port );
            var landingUrl = new UriBuilder( m_scheme, m_host, m_port, RouteProvider.OrganizationInfoRoute ).Uri;
			var uri = m_appContext.CreateUrlForAuthentication( hostSpec, landingUrl );

            var request = RequestProvider.CreateRequest( uri );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
			}
		}
		
		[Test]
		public void SendAuthenticatedRequest_ResponseReceived() {
            var request = RequestProvider.PrepareApiRequest( m_userContext, RouteProvider.OrganizationInfoRoute );

			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendAuthenticatedRequest_StatusCodeIs200() {
            var request = RequestProvider.PrepareApiRequest( m_userContext, RouteProvider.OrganizationInfoRoute );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
			}
			Assert.DoesNotThrow( () => request.GetResponse() );
		}

		[Test]
		public void SendAuthenticatedRequest_ResponseContentsIsNotEmpty() {
            var request = RequestProvider.PrepareApiRequest( m_userContext, RouteProvider.OrganizationInfoRoute );

			using( var response = request.GetResponse() as HttpWebResponse ) {
                string contents = StringHelper.ReadResponseContents( response );
				Assert.IsNotNullOrEmpty( contents );
			}
		}
		
		[Test]
		public void SendAuthenticatedRequest_ResponseContents_CanBeDeserializedAsJson() {
            var request = RequestProvider.PrepareApiRequest( m_userContext, RouteProvider.OrganizationInfoRoute );

			using( var response = request.GetResponse() as HttpWebResponse ) {
				var org = StringHelper.DeserializeResponseContents<Organization>( response );
				Assert.IsNotNull( org );
			}
		}

		
		[Test]
		public void SendAuthenticatedRequestWithDelayedTimestamp_ResponseMustContainTimeOffset() {

			m_appContext = CreateAppContextWithDelay( m_appId, m_appKey, TEST_TIME_DELAY );

			var userContext = CreateUserOperationContext();
            var uri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );

			var request = (HttpWebRequest) WebRequest.Create( uri );
			request.Method = "GET";

			try {
				request.GetResponse();
			} catch( WebException ex ) {
                string responseContents = StringHelper.ReadResponseContents( ex.Response as HttpWebResponse );

				Assert.That( responseContents, Is.StringMatching( "Timestamp out of range\\s?(\\d+)" )  );
				return;
			}
			Assert.Fail( "Expected WebException was not thrown");
		}

		[Test]
		public void RetryAuthenticatedRequestWithCorrectedTimestamp_ResultCodeIs200() {

			m_appContext = CreateAppContextWithDelay( m_appId, m_appKey, TEST_TIME_DELAY );

			var userContext = CreateUserOperationContext();
            var uri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );
			var request = (HttpWebRequest) WebRequest.Create( uri );
			request.Method = "GET";
			try {
				request.GetResponse();
			}
			catch( WebException ex ) {
				var exWrapper = new D2LWebException( ex );
				userContext.InterpretResult( exWrapper );
			}
            var retryUri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );
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

        private void ReadConfig() {
            m_appId = ConfigurationManager.AppSettings["appId"];
            m_appKey = ConfigurationManager.AppSettings["appKey"];
            m_userId = ConfigurationManager.AppSettings["userId"];
            m_userKey = ConfigurationManager.AppSettings["userKey"];
            m_scheme = ConfigurationManager.AppSettings["scheme"];
            m_host = ConfigurationManager.AppSettings["host"];
            m_port = Int32.Parse( ConfigurationManager.AppSettings["port"] );
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

		private const string GET_VERSIONS_ROUTE = "/d2l/api/versions/";
		private const long TEST_TIME_DELAY = 600L;
	}
}
