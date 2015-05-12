using System;
using System.Net;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;
using D2L.Extensibility.AuthSdk.IntegrationTests.Providers;
using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {

	[TestFixture]
	public class LearningTests {

		private HttpWebRequest m_request;

		[SetUp]
		public void TestSetup() {
			m_request = RequestProvider.PrepareApiRequest( ContextProvider.BadUserContext(), RouteProvider.OrganizationInfoRoute );
		}

		[Test]
		public void LearningTest_SendRequestWithBadKeys_ThrowsWebException() {
			Assert.Throws<WebException>( () => { using( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) { } } );
		}

		[Test]
		public void LearningTest_SendRequestWithBadKeys_ResponseBodyContains_Invalid_token() {
			string responseBody = string.Empty;

			try {
				using( HttpWebResponse response = m_request.GetResponse() as HttpWebResponse ) { }
			} catch( WebException ex ) {
				responseBody = StringHelper.ReadResponseContents( ex.Response as HttpWebResponse );
			}

			Assert.IsTrue( responseBody.Equals( "Invalid token", StringComparison.InvariantCulture ) );
		}
	}
}
