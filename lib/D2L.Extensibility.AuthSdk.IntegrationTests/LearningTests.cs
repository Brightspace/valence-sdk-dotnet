using System;
using System.Net;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;
using D2L.Extensibility.AuthSdk.IntegrationTests.Providers;
using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {

    [TestFixture]
    public class LearningTests {

        [Test]
        public void LearningTest_SendRequestWithBadKeys_ThrowsWebException() {
            var request = RequestProvider.PrepareApiRequest( ContextProvider.BadUserContext(), RouteProvider.OrganizationInfoRoute );

            Assert.Throws<WebException>( () => request.GetResponse() );
        }

        [Test]
        public void LearningTest_SendRequestWithBadKeys_ResponseBodyContains_Invalid_token() {
            var request = RequestProvider.PrepareApiRequest( ContextProvider.BadUserContext(), RouteProvider.OrganizationInfoRoute );

            try {
                request.GetResponse();
            } catch ( WebException ex ) {
                var responseBody = StringHelper.ReadResponseContents( ex.Response as HttpWebResponse );
                Assert.IsTrue( responseBody.Equals( "Invalid token", StringComparison.InvariantCulture ) );
            }
        }
    }
}
