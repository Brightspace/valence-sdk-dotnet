using System.Net;
using D2L.Extensibility.AuthSdk.IntegrationTests.Domain;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;
using D2L.Extensibility.AuthSdk.IntegrationTests.Providers;
using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {

    [TestFixture]
    public class AnonymousRequestTests {

        private HttpWebRequest m_request;

        [SetUp]
        public void TestSetup() {
            m_request = RequestProvider.PrepareApiRequest( ContextProvider.AnonUserContext(), RouteProvider.VersionsRoute );
        }

        [Test]
        public void SendAnonymousRequest_ResponseReceived() {
            Assert.DoesNotThrow( () => { using ( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) { } } );
        }

        [Test]
        public void SendAnonymousRequest_StatusCodeIs200() {
            using ( var response = m_request.GetResponse() as HttpWebResponse ) {
                Assert.AreEqual( HttpStatusCode.OK, response.StatusCode );
            }

            Assert.DoesNotThrow( () => m_request.GetResponse() );
        }

        [Test]
        public void SendAnonymousRequest_ResponseContentsIsNotEmpty() {
            string contents;

            using ( var response = m_request.GetResponse() as HttpWebResponse ) {
                contents = StringHelper.ReadResponseContents( response );
            }

            Assert.IsNotNullOrEmpty( contents );
        }

        [Test]
        public void SendAnonymousRequest_ResponseContents_CanBeDeserializedAsJson() {
            ProductVersions[] versions;

            using ( var response = m_request.GetResponse() as HttpWebResponse ) {
                versions = StringHelper.DeserializeResponseContents<ProductVersions[]>( response );
            }

            Assert.IsNotNull( versions );
        }
    }
}
