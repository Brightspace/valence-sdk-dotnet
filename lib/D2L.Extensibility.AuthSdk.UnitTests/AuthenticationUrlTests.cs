using System;
using System.Web;
using System.Text.RegularExpressions;
using NUnit.Framework;

using D2L.Extensibility.AuthSdk.Impl;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	[TestFixture]
	public class AuthenticationUrlTests {
		private ID2LAppContext m_appContext;
		private HostSpec m_testHostSpec;
		private Uri m_landingUri;

		[OneTimeSetUp]
		public void SetUpTestObjects() {
			m_testHostSpec = new HostSpec( "https", TestConstants.HOST_NAME, TestConstants.PORT );
			m_landingUri = new Uri( TestConstants.API_URL );
		}

		[SetUp]
		public void SetUpAppContext() {
			m_appContext = TestUtils.CreateAppContextUnderTest();
		}

		[Test]
		public void AppContext_CreateUrlForAuth_IfHostSpecSaysHttp_ReturnsUri_WhereSchemeIsHttp() {
			var hostSpec = new HostSpec( "http", TestConstants.HOST_NAME, 80 );

			Uri uri = m_appContext.CreateUrlForAuthentication( hostSpec, m_landingUri );

			Assert.AreEqual( "http", uri.Scheme );
		}

		[Test]
		public void AppContext_CreateUrlForAuth_IfHostSpecSaysHttps_ReturnsUri_WhereSchemeIsHttps() {
			var hostSpec = new HostSpec( "https", TestConstants.HOST_NAME, 443 );

			Uri uri = m_appContext.CreateUrlForAuthentication( hostSpec, m_landingUri );

			Assert.AreEqual( "https", uri.Scheme );
		}

		[Test]
		public void AppContext_CreateUrlForAuth_RespectsURIEncodingInCallbackURL() {
			var hostSpec = new HostSpec( "https", TestConstants.HOST_NAME, 443 );
			Uri uri = m_appContext.CreateUrlForAuthentication( hostSpec, new Uri(TestConstants.ESCAPED_CALLBACK) );

			var regex = new Regex(@"x_target=([^&]*)");
			var match = regex.Match( uri.AbsoluteUri );
			Assert.IsTrue( match.Success );
			string val = match.Groups[1].Value;

			Assert.AreEqual(Uri.EscapeDataString(TestConstants.ESCAPED_CALLBACK).ToLower(), val.ToLower(), "Full Uri: " + uri.AbsoluteUri);
		}

		[Test]
		public void AppContext_CreateUrlForAuth_ReturnsUri_WhereHost_MatchesInput() {
			const string expectedHost = "asdf.com";
			var hostSpec = new HostSpec( "https", expectedHost, TestConstants.PORT );

			Uri uri = m_appContext.CreateUrlForAuthentication( hostSpec, m_landingUri );

			Assert.AreEqual( expectedHost, uri.Host );
		}

		[Test]
		public void AppContext_CreateUrlForAuth_ReturnsUri_WherePortNumber_MatchesInput() {
			const int expectedPortNumber = 44480;
			var hostSpec = new HostSpec( "https", TestConstants.HOST_NAME, expectedPortNumber );

			Uri uri = m_appContext.CreateUrlForAuthentication( hostSpec, m_landingUri );

			Assert.AreEqual( expectedPortNumber, uri.Port );
		}
		
		[Test]
		public void AppContext_CreateUrlForAuth_ReturnsUri_WithPath_ToTokenService() {
			Uri uri = m_appContext.CreateUrlForAuthentication( m_testHostSpec, m_landingUri );

			Assert.AreEqual( EXPECTED_TOKEN_SERVICE_PATH, uri.AbsolutePath );
		}

		[Test]
		public void AppContext_CreateUrlForAuth_ReturnsUri_AndQueryParam_x_a_Matches_AppId() {
			Uri uri = m_appContext.CreateUrlForAuthentication( m_testHostSpec, m_landingUri );

			string parameter = GetUriQueryParameter( uri, "x_a" );
			Assert.AreEqual( TestConstants.APP_ID, parameter );
		}

		[Test]
		public void AppContext_CreateUrlForAuth_ReturnsUri_AndQueryParam_x_b_MatchesLandingUriSignedWithAppKey() {
			string expectedSignedUri = D2LSigner.GetBase64HashString(
				TestConstants.APP_KEY, TestConstants.API_URL );
			
			Uri uri = m_appContext.CreateUrlForAuthentication( m_testHostSpec, m_landingUri );

			string parameter = GetUriQueryParameter( uri, "x_b" );
			Assert.AreEqual( expectedSignedUri, parameter );
		}

		[Test]
		public void AppContext_CreateUrlForAuth_ReturnsUri_AndQueryParam_x_target_MatchesLandingUrl() {
			string expectedTarget = HttpUtility.UrlEncode( TestConstants.API_URL );

			Uri uri = m_appContext.CreateUrlForAuthentication( m_testHostSpec, m_landingUri );

			string parameter = GetUriQueryParameter( uri, "x_target" );
			Assert.AreEqual( expectedTarget, parameter );
		}

		[Test]
		public void AppContext_CreateUrlForAuth_ReturnsUri_AndQueryParam_type_IsAbsent() {
			Uri uri = m_appContext.CreateUrlForAuthentication( m_testHostSpec, m_landingUri );

			Assert.Throws<ArgumentException>( () => GetUriQueryParameter( uri, "type" ) );
		}

		[Test]
		public void AppContext_CreateWebUrlForAuth_LandingUriHasSpecialChars_ReturnsUri_AndQueryParam_x_target_MatchesEncodedLandingUrl() {
			const string unencodedUrl = "http://univ.edu/d2l/api/resource?foo=bar";
			string encodedUrl = HttpUtility.UrlEncode( unencodedUrl );
			var hostSpec = new HostSpec( "https", TestConstants.HOST_NAME, TestConstants.PORT );

			Uri uri = m_appContext.CreateUrlForAuthentication( hostSpec, new Uri( unencodedUrl ) );

			string parameter = GetUriQueryParameter( uri, "x_target" );
			Assert.AreEqual( encodedUrl, parameter );
		}
		
		private static string GetUriQueryParameter( Uri uri, string name ) {
			string queryString = uri.Query.Substring( 1 );
			string[] nameValuePairStrings = queryString.Split( new[] { '&' } );
			foreach( string pairString in nameValuePairStrings ) {
				string[] nameValuePair = pairString.Split( new[] { '=' } );
				if( nameValuePair.Length >= 2 && nameValuePair[0].Equals( name ) ) {
					return nameValuePair[1];
				}
			}
			throw new ArgumentException( "didn't find query parameter " + name );
		}

		private const string EXPECTED_TOKEN_SERVICE_PATH = "/d2l/auth/api/token";
	}
}
