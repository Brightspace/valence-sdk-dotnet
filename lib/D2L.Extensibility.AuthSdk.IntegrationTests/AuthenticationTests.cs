using System;
using System.Net;
using System.Threading;
using D2L.Extensibility.AuthSdk;
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
            using ( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) {
                Organization org = StringHelper.DeserializeResponseContents<Organization>( response );
                Assert.IsNotNull( org );
            }
        }

        [Test]
        public void SendAuthenticatedRequest_ResponseContents_IsNotEmpty() {
            using ( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) {
                string contents = StringHelper.ReadResponseContents( response );
                Assert.IsNotNullOrEmpty( contents );
            }
        }

        [Test]
        public void SendAuthenticatedRequest_ResponseReceived() {
            Assert.DoesNotThrow( () => { using ( m_request.GetResponse() as HttpWebResponse ) { } } );
        }

        [Test]
        public void SendAuthenticatedRequest_StatusCodeIs200() {
            using ( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) {
                Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
            }
        }

        [Test]
        public void SendRequest_AtUrlForAuthentication_StatusCodeIs200() {
            Uri landingUrl = new UriBuilder( ConfigHelper.Scheme, ConfigHelper.Host, ConfigHelper.Port, RouteProvider.OrganizationInfoRoute ).Uri;
            Uri uri = ContextProvider.AuthenticatedUrl( landingUrl );

            HttpWebRequest request = RequestProvider.CreateRequest( uri );

            using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) {
                Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
            }
        }

        [Test]
        public void SendRequest_AtUrlForAuthentication_ResponseReceived() {
            Uri landingUrl = new UriBuilder( ConfigHelper.Scheme, ConfigHelper.Host, ConfigHelper.Port, RouteProvider.OrganizationInfoRoute ).Uri;
            Uri uri = ContextProvider.AuthenticatedUrl( landingUrl );

            HttpWebRequest request = RequestProvider.CreateRequest( uri );

            Assert.DoesNotThrow( () => { using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) { } } );
        }

        [Test]
        public void SendAuthenticatedRequestWithDelayedTimestamp_ResponseMustContainTimeOffset() {
            ID2LAppContext appContext = CreateAppContextWithDelay( ConfigHelper.AppId, ConfigHelper.AppKey, TEST_TIME_DELAY );
            ID2LUserContext userContext = ContextProvider.UserContext( appContext );
            Uri uri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create( uri );
            request.Method = "GET";

            try {
                using ( request.GetResponse() as HttpWebResponse ) { }
            } catch ( WebException ex ) {
                string responseContents = StringHelper.ReadResponseContents( ex.Response as HttpWebResponse );

                Assert.That( responseContents, Is.StringMatching( "Timestamp out of range\\s?(\\d+)" ) );
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
                using ( request.GetResponse() as HttpWebResponse ) { }
            } catch ( WebException ex ) {
                var exWrapper = new D2LWebException( ex );
                userContext.InterpretResult( exWrapper );
            }

            Uri retryUri = userContext.CreateAuthenticatedUri( RouteProvider.OrganizationInfoRoute, "GET" );
            HttpWebRequest retryRequest = (HttpWebRequest)WebRequest.Create( retryUri );
            retryRequest.Method = "GET";

            Assert.DoesNotThrow( () => { using ( HttpWebResponse response = retryRequest.GetResponse() as HttpWebResponse ) { } } );
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
