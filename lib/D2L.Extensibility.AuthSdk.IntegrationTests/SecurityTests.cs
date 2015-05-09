using System.Net;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;
using D2L.Extensibility.AuthSdk.IntegrationTests.Providers;
using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {
    
    [TestFixture]
    public class SecurityTests {

        [Test]
        public void SendRequestWithBadKeys_ResponseInterpretationIs_InvalidSig() {
            ID2LUserContext badContext = ContextProvider.BadUserContext();
            HttpWebRequest request = RequestProvider.PrepareApiRequest( badContext, RouteProvider.OrganizationInfoRoute );

            try {
                using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) { }
            } catch ( WebException ex ) {
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

            Assert.Throws<WebException>( () => { using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse ) { } } );
        }

        private string ChangeScheme( string scheme ) {
            return scheme == "https" ? "http" : "https";
        }
    }
}
