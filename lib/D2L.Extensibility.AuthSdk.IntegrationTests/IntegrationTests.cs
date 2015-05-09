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



		

		private HttpWebRequest m_asyncRequest;
		private HttpStatusCode m_asyncResponseStatusCode;
		private EventWaitHandle m_getResponseAsyncCallbackHandle;













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
	}
}
