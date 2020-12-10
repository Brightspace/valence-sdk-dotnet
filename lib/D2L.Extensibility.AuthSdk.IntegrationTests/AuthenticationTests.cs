using System;
using System.Net;
using D2L.Extensibility.AuthSdk.IntegrationTests.Domain;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;
using D2L.Extensibility.AuthSdk.IntegrationTests.Providers;
using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {

	[TestFixture]
	public class AuthenticationTests {

		private const long TEST_TIME_DELAY = 600L;

		private HttpWebRequest m_request;

		[SetUp]
		public void TestSetup() {
			m_request = RequestProvider.PrepareApiRequest( ContextProvider.UserContext(), RouteProvider.OrganizationInfoRoute );
		}

		[Test]
		public void SendAuthenticatedRequest_ResponseContents_CanBeDeserializedAsJson() {
			using( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) {
				Organization org = StringHelper.DeserializeResponseContents<Organization>( response );
				Assert.IsNotNull( org );
			}
		}

		[Test]
		public void SendAuthenticatedRequest_ResponseContents_IsNotEmpty() {
			using( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) {
				string contents = StringHelper.ReadResponseContents( response );
				Assert.IsNotNull( contents );
			}
		}

		[Test]
		public void SendAuthenticatedRequest_ResponseReceived() {
			Assert.DoesNotThrow( () => { using( m_request.GetResponse() as HttpWebResponse ) { } } );
		}

		[Test]
		public void SendAuthenticatedRequest_StatusCodeIs200() {
			using( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) {
				Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
			}
		}

		[Test]
		public void SendRequest_AtUrlForAuthentication_StatusCodeIs200() {
			Uri landingUrl = new UriBuilder( ConfigHelper.Scheme, ConfigHelper.Host, ConfigHelper.Port, RouteProvider.OrganizationInfoRoute ).Uri;
			Uri uri = ContextProvider.AuthenticatedUrl( landingUrl );

			HttpWebRequest request = RequestProvider.CreateRequest( uri );

			using( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) {
				Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
			}
		}

		[Test]
		public void SendRequest_AtUrlForAuthentication_ResponseReceived() {
			Uri landingUrl = new UriBuilder( ConfigHelper.Scheme, ConfigHelper.Host, ConfigHelper.Port, RouteProvider.OrganizationInfoRoute ).Uri;
			Uri uri = ContextProvider.AuthenticatedUrl( landingUrl );

			HttpWebRequest request = RequestProvider.CreateRequest( uri );

			Assert.DoesNotThrow( () => { using( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) { } } );
		}

		[Test]
		public void SendAuthenticatedRequestWithDelayedTimestamp_ResponseMustContainTimeOffset() {
			ID2LAppContext appContext = CreateAppContextWithDelay( ConfigHelper.AppId, ConfigHelper.AppKey, TEST_TIME_DELAY );
			ID2LUserContext userContext = ContextProvider.UserContext( appContext );
			Uri uri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create( uri );
			request.Method = "GET";

			try {
				using( request.GetResponse() as HttpWebResponse ) { }
			} catch( WebException ex ) {
				string responseContents = StringHelper.ReadResponseContents( ex.Response as HttpWebResponse );

				StringAssert.IsMatch( "Timestamp out of range\\s?(\\d+)", responseContents );
				return;
			}
			Assert.Fail( "Expected WebException was not thrown" );
		}

		public void RetryAuthenticatedRequestWithCorrectedTimestamp_ResultCodeIs200() {
			ID2LAppContext appContext = CreateAppContextWithDelay( ConfigHelper.AppId, ConfigHelper.AppKey, TEST_TIME_DELAY );
			ID2LUserContext userContext = ContextProvider.UserContext( appContext );
			Uri uri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create( uri );
			request.Method = "GET";

			try {
				using( request.GetResponse() as HttpWebResponse ) { }
			} catch( WebException ex ) {
				var exWrapper = new D2LWebException( ex );
				userContext.InterpretResult( exWrapper );
			}

			Uri retryUri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );
			HttpWebRequest retryRequest = (HttpWebRequest)WebRequest.Create( retryUri );
			retryRequest.Method = "GET";

			Assert.DoesNotThrow( () => { using( HttpWebResponse response = retryRequest.GetResponse() as HttpWebResponse ) { } } );
		}

		[Test]
		public void SendRequestWithBadKeys_ResponseInterpretationIs_InvalidSig() {
			ID2LUserContext badContext = ContextProvider.BadUserContext();
			HttpWebRequest request = RequestProvider.PrepareApiRequest( badContext, RouteProvider.OrganizationInfoRoute );

			try {
				using( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) { }
			} catch( WebException ex ) {
				var exceptionWrapper = new D2LWebException( ex );
				var interpretation = badContext.InterpretResult( exceptionWrapper );
				Assert.AreEqual( RequestResult.RESULT_INVALID_SIG, interpretation );
			}
		}

		[Test]
		public void SendRequest_WhenBadHostSpec_UnhandledException() {
			HostSpec badApiHost = new HostSpec( ChangeScheme( ConfigHelper.Scheme ), ConfigHelper.Host, ConfigHelper.Port );
			ID2LUserContext badAnonContext = ContextProvider.AnonUserContext( badApiHost );

			HttpWebRequest request = RequestProvider.PrepareApiRequest( badAnonContext, RouteProvider.VersionsRoute );

			Assert.Throws<WebException>( () => { using( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) { } } );
		}

		private string ChangeScheme( string scheme ) {
			return scheme == "https" ? "http" : "https";
		}

		private static ID2LAppContext CreateAppContextWithDelay( string appId, string appKey, long delaySeconds ) {

			var timestampProvider = CreateTimestampProviderDelay( delaySeconds );
			var factory = new D2LAppContextFactory( timestampProvider );
			return factory.Create( appId, appKey );
		}

		private static ITimestampProvider CreateTimestampProviderDelay( long seconds ) {
			return new LateTimestampProvider( seconds );
		}
	}
}
