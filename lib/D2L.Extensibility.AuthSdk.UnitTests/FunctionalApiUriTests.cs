using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	[TestFixture]
	public class FunctionalApiUriTests {
		private ID2LAppContext m_appContext;
		private ID2LUserContext m_userContext;
		private ID2LUserContext m_anonContext;

		[SetUp]
		public void SetUpAppContext() {
			m_appContext = TestUtils.CreateAppContextUnderTest();
			TestUtils.SetUpTimestampProviderStub( TestConstants.TIMESTAMP_MILLISECONDS );
			m_userContext = CreateUserContextUnderTest();
			m_anonContext = CreateAnonymousContextUnderTest();
		}

		[Test]
		public void UserContext_CreateAuthUri_ResultQueryParam_x_a_IsSameAs_AppId() {
			Uri authUri = m_userContext.CreateAuthenticatedUri( TestConstants.API_PATH, "PUT" );

			string parameter = TestUtils.GetUriQueryParameter( authUri, "x_a" );
			Assert.AreEqual( TestConstants.APP_ID, parameter );
		}

		[Test]
		public void UserContext_CreateAuthUri_ResultQueryParam_x_c_Matches_SignatureSignedWithAppKey() {
			const string httpMethod = "POST";
			string expectedParameter = TestUtils.CalculateParameterExpectation(
				TestConstants.APP_KEY, httpMethod,
				TestConstants.API_PATH, TestConstants.TIMESTAMP_SECONDS );

			Uri authUri = m_userContext.CreateAuthenticatedUri( TestConstants.API_PATH, httpMethod );

			string parameter = TestUtils.GetUriQueryParameter( authUri, "x_c" );
			Assert.AreEqual( expectedParameter, parameter );
		}

		[Test]
		public void UserContext_CreateAuthUri_ResultQueryParam_x_b_Matches_UserId() {
			Uri authUri = m_userContext.CreateAuthenticatedUri( TestConstants.API_PATH, "GET" );

			string parameter = TestUtils.GetUriQueryParameter( authUri, "x_b" );
			Assert.AreEqual( TestConstants.USER_ID, parameter );
		}

		[Test]
		public void UserContext_CreateAuthUri_ResultQueryParam_x_d_Matches_SignatureSignedWithUserKey() {
			const string httpMethod = "GET";
			string expectedParameter = TestUtils.CalculateParameterExpectation(
				TestConstants.USER_KEY, httpMethod,
				TestConstants.API_PATH, TestConstants.TIMESTAMP_SECONDS );

			Uri authUri = m_userContext.CreateAuthenticatedUri( TestConstants.API_PATH, httpMethod );

			string parameter = TestUtils.GetUriQueryParameter( authUri, "x_d" );
			Assert.AreEqual( expectedParameter, parameter );
		}


		[Test]
		public void UserContext_CreateAuthUri_ResultQueryParam_x_t_MatchesAdjustedTimestampInSeconds() {
			const string httpMethod = "PUT";
			const long serverClockSkewMilliseconds = 225000;
			TestUtils.SetUpTimestampProviderStub(
				TestConstants.TIMESTAMP_MILLISECONDS - serverClockSkewMilliseconds );
			var userContext = CreateUserContextUnderTest();
			userContext.ServerSkewMillis = serverClockSkewMilliseconds;

			Uri authUri = userContext.CreateAuthenticatedUri( TestConstants.API_PATH, httpMethod );

			string expectedTimestampParameter =
				TestConstants.TIMESTAMP_SECONDS.ToString( CultureInfo.InvariantCulture );
			string actualTimestampParameter = TestUtils.GetUriQueryParameter( authUri, "x_t" );
			Assert.AreEqual( expectedTimestampParameter, actualTimestampParameter );
		}

		[Test]
		public void UserContext_CreateAuthUri_SchemeMatches() {
			const string expectedScheme = "spdy";
			var apiHost = new HostSpec( expectedScheme, TestConstants.HOST_NAME, TestConstants.PORT );
			var userContext = m_appContext.CreateUserContext(
				TestConstants.USER_ID, TestConstants.USER_KEY, apiHost );

			Uri authUri = userContext.CreateAuthenticatedUri( TestConstants.API_PATH, "GET" );

			Assert.AreEqual( expectedScheme, authUri.Scheme );
		}

		[Test]
		public void UserContext_CreateAuthUri_HostNameMatches() {
			const string expectedHost = "myuniv.edu";
			var apiHost = new HostSpec( "https", expectedHost, TestConstants.PORT );
			var userContext = m_appContext.CreateUserContext(
				TestConstants.USER_ID, TestConstants.USER_KEY, apiHost );

			Uri authUri = userContext.CreateAuthenticatedUri( TestConstants.API_PATH, "GET" );

			Assert.AreEqual( expectedHost, authUri.Host );
		}

		[Test]
		public void UserContext_CreateAuthUri_PortMatches() {
			const int expectedPort = 1905;
			var apiHost = new HostSpec( "https", TestConstants.HOST_NAME, expectedPort );
			var userContext = m_appContext.CreateUserContext(
				TestConstants.USER_ID, TestConstants.USER_KEY, apiHost );

			Uri authUri = userContext.CreateAuthenticatedUri( TestConstants.API_PATH, "GET" );

			Assert.AreEqual( expectedPort, authUri.Port );
		}

		[Test]
		public void AnonymousUserContext_CreateAuthUri_ResultQueryParam_x_a_IsSameAs_AppId() {
			Uri authUri = m_anonContext.CreateAuthenticatedUri( TestConstants.API_PATH, "PUT" );

			string parameter = TestUtils.GetUriQueryParameter( authUri, "x_a" );
			Assert.AreEqual( TestConstants.APP_ID, parameter );
		}

		[Test]
		public void AnonymousUserContext_CreateAuthUri_ResultQueryParam_x_c_Matches_SignatureSignedWithAppKey() {
			const string httpMethod = "POST";
			string expectedParameter = TestUtils.CalculateParameterExpectation(
				TestConstants.APP_KEY, httpMethod,
				TestConstants.API_PATH, TestConstants.TIMESTAMP_SECONDS );

			Uri authUri = m_anonContext.CreateAuthenticatedUri( TestConstants.API_PATH, httpMethod );

			string parameter = TestUtils.GetUriQueryParameter( authUri, "x_c" );
			Assert.AreEqual( expectedParameter, parameter );
		}

		[Test]
		public void AnonymousUserContext_CreateAuthUri_ResultQueryParam_x_b_Matches_UserId() {
			Uri authUri = m_anonContext.CreateAuthenticatedUri( TestConstants.API_PATH, "GET" );

			Assert.Throws<ArgumentException>( () => TestUtils.GetUriQueryParameter( authUri, "x_b" ) );
		}

		[Test]
		public void AnonymousUserContext_CreateAuthUri_ResultQueryParam_x_d_Matches_SignatureSignedWithUserKey() {
			const string httpMethod = "GET";

			Uri authUri = m_anonContext.CreateAuthenticatedUri( TestConstants.API_PATH, httpMethod );

			Assert.Throws<ArgumentException>( () => TestUtils.GetUriQueryParameter( authUri, "x_d" ) );
		}

		[Test]
		public void AnonymousUserContext_CreateAuthUri_ResultQueryParam_x_t_MatchesAdjustedTimestampInSeconds() {
			const string httpMethod = "PUT";
			const long serverClockSkewMilliseconds = 213000;
			TestUtils.SetUpTimestampProviderStub(
				TestConstants.TIMESTAMP_MILLISECONDS - serverClockSkewMilliseconds );
			var anonContext = CreateAnonymousContextUnderTest();
			anonContext.ServerSkewMillis = serverClockSkewMilliseconds;

			Uri authUri = anonContext.CreateAuthenticatedUri( TestConstants.API_PATH, httpMethod );

			string expectedTimestampParameter =
				TestConstants.TIMESTAMP_SECONDS.ToString( CultureInfo.InvariantCulture );
			string actualTimestampParameter = TestUtils.GetUriQueryParameter( authUri, "x_t" );
			Assert.AreEqual( expectedTimestampParameter, actualTimestampParameter );
		}

		[Test]
		public void UserContext_CreateAuthUri_Ignores_URI_Escaping_When_Signing() {
			String first = m_userContext.CreateAuthenticatedUri( "/d2l/api/this is a test", "GET" ).OriginalString;
			String second = m_userContext.CreateAuthenticatedUri( "/d2l/api/this%20is%20a%20test", "GET" ).OriginalString;
			first = first.Replace( " ", "%20" ); // still need to url encode the route
			Assert.AreEqual( first, second ); // but their signatures should match
		}

		[Test]
		public void UserContext_CreateAuthUri_VerbCase_DoesNotChangeResult() {
			Uri expectedUri = m_userContext.CreateAuthenticatedUri( TestConstants.API_PATH, "GET" );

			Uri actualUri = m_userContext.CreateAuthenticatedUri( TestConstants.API_PATH, "Get" );

			Assert.AreEqual( expectedUri.ToString(), actualUri.ToString() );
		}

		[Test]
		public void UserContext_CreateAuthUri_PathCase_DoesNotChangeResult() {
			Uri expectedUri = m_userContext.CreateAuthenticatedUri( "/d2l/api/someresource", "GET" );
			string expectedParameter = TestUtils.GetUriQueryParameter( expectedUri, "x_c" );

			Uri actualUri = m_userContext.CreateAuthenticatedUri( "/d2l/api/SomeResource", "GET" );

			string actualParameter = TestUtils.GetUriQueryParameter( actualUri, "x_c" );
			Assert.AreEqual( expectedParameter, actualParameter );
		}

		[Test]
		public void UserContext_CreateAuthUri_IfThereIsClockSkew_SignatureUsesAdjustedTimestamp() {
			const string httpMethod = "POST";
			const long serverClockSkewMilliseconds = 343000;
			string expectedParameter = TestUtils.CalculateParameterExpectation(
				TestConstants.APP_KEY, httpMethod,
				TestConstants.API_PATH, TestConstants.TIMESTAMP_SECONDS );
			TestUtils.SetUpTimestampProviderStub( TestConstants.TIMESTAMP_MILLISECONDS - serverClockSkewMilliseconds );
			var userContext = CreateUserContextUnderTest();
			userContext.ServerSkewMillis = serverClockSkewMilliseconds;

			Uri authUri = userContext.CreateAuthenticatedUri( TestConstants.API_PATH, httpMethod );

			string parameter = TestUtils.GetUriQueryParameter( authUri, "x_c" );
			Assert.AreEqual( expectedParameter, parameter );
		}
		
		private ID2LUserContext CreateUserContextUnderTest() {
			var testApiHost = TestUtils.CreateTestHost();
			return m_appContext.CreateUserContext(
				TestConstants.USER_ID, TestConstants.USER_KEY, testApiHost );
		}

		private ID2LUserContext CreateAnonymousContextUnderTest() {
			var testApiHost = TestUtils.CreateTestHost();
			return m_appContext.CreateAnonymousUserContext( testApiHost );
		}

	}
}
