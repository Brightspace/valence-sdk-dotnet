using System;

using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	[TestFixture]
	public class UserContextCreationTests {
		private ID2LAppContext m_appContext;
		private Uri m_authCallbackUri;
		private HostSpec m_apiHost;

		[TestFixtureSetUp]
		public void SetUpTestObjects() {
			m_authCallbackUri = TestUtils.CreateTestAuthenticationCallbackUri(
				TestConstants.USER_ID, TestConstants.USER_KEY );
			m_apiHost = new HostSpec( "https", TestConstants.HOST_NAME, TestConstants.PORT );
		}

		[SetUp]
		public void SetUpAppContext() {
			m_appContext = TestUtils.CreateAppContextUnderTest();
		}

		[Test]
		public void AppContext_CreateUserContext_ReturnedObject_ImplementsID2LUserContext() {
			var result = m_appContext.CreateUserContext( m_authCallbackUri, m_apiHost );

			Assert.IsInstanceOf<ID2LUserContext>( result );
		}

		[Test]
		public void AppContext_CreateUserContext_FromAuthCallbackUri_IfNo_x_a_QueryParam_ReturnsNull() {
			var parameterlessUri = new Uri( TestConstants.API_URL );
			var userContext = m_appContext.CreateUserContext( parameterlessUri, m_apiHost );

			Assert.IsNull( userContext );
		}

		[Test]
		public void AppContext_CreateUserContext_FromAuthCallbackUri_IfNo_x_b_QueryParam_ReturnsNull() {
			Uri resultUri = TestUtils.CreateTestAuthenticationCallbackUri( TestConstants.USER_ID, "" );

			var userContext = m_appContext.CreateUserContext( resultUri, m_apiHost );

			Assert.IsNull( userContext );
		}

		[Test]
		public void AppContext_CreateUserContext_FromAuthCallbackUri_SaveProps_UserId_Matches_x_a_QueryParam() {
			const string expectedUserId = "344";
			Uri authCallbackUri = TestUtils.CreateTestAuthenticationCallbackUri(
				expectedUserId, TestConstants.USER_KEY );

			var userContext = m_appContext.CreateUserContext( authCallbackUri, m_apiHost );
			var savedProps = userContext.SaveUserContextProperties();

			Assert.AreEqual( expectedUserId, savedProps.UserId );
		}

		[Test]
		public void AppContext_CreateUserContext_FromAuthCallbackUri_SavePropsUserKey_Matches_x_b_QueryParam() {
			const string expectedUserKey = "sampleUserKey";
			Uri resultUri = TestUtils.CreateTestAuthenticationCallbackUri(
				TestConstants.USER_ID, expectedUserKey );

			var userContext = m_appContext.CreateUserContext( resultUri, m_apiHost );
			var savedProps = userContext.SaveUserContextProperties();

			Assert.AreEqual( expectedUserKey, savedProps.UserKey );
		}

		[Test]
		public void AppContext_CreateUserContext_FromAuthCallbackUri_SaveProps_HostMatches() {
			const string expectedHost = "univ.edu";
			var apiHost = new HostSpec( "https", expectedHost, 443 );

			var userContext = m_appContext.CreateUserContext( m_authCallbackUri, apiHost );
			var savedProps = userContext.SaveUserContextProperties();

			Assert.AreEqual( expectedHost, savedProps.HostName );
		}

		[Test]
		public void AppContext_CreateUserContext_FromAuthCallbackUri_SaveProps_PortMatches() {
			const int expectedPort = 2228;
			var apiHost = new HostSpec( "https", TestConstants.HOST_NAME, expectedPort );

			var userContext = m_appContext.CreateUserContext( m_authCallbackUri, apiHost );
			var savedProps = userContext.SaveUserContextProperties();

			Assert.AreEqual( expectedPort, savedProps.Port );
		}

		[Test]
		public void AppContext_CreateUserContext_FromAuthCallbackUri_SaveProps_SchemeMatches() {
			const string expectedScheme = "foo";
			var apiHost = new HostSpec( expectedScheme, TestConstants.HOST_NAME, TestConstants.PORT );

			var userContext = m_appContext.CreateUserContext( m_authCallbackUri, apiHost );
			var savedProps = userContext.SaveUserContextProperties();

			Assert.AreEqual( expectedScheme, savedProps.Scheme );
		}

		[Test]
		public void AppContext_CreateUserSpecificContext_FromSavedProperties_SaveAgain_PropertiesMatch() {
			var originalUserContext = m_appContext.CreateUserContext( m_authCallbackUri, m_apiHost );
			var savedProps = originalUserContext.SaveUserContextProperties();

			var restoredContext = m_appContext.CreateUserContext( savedProps );
			var savedAgainProps = restoredContext.SaveUserContextProperties();

			Assert.IsTrue( savedProps.EqualTo( savedAgainProps ) );
		}

		[Test]
		public void AppContext_CreateUserContext_WithUserIdAndKey_SavedProps_MatchThose_FromAuthCallback() {
			var originalUserContext = m_appContext.CreateUserContext( m_authCallbackUri, m_apiHost );
			var oldProps = originalUserContext.SaveUserContextProperties();

			var newContext = m_appContext.CreateUserContext(
				TestConstants.USER_ID, TestConstants.USER_KEY, m_apiHost );
			var newProps = newContext.SaveUserContextProperties();

			Assert.IsTrue( oldProps.EqualTo( newProps ) );
		}

		[Test]
		public void AppContext_CreateAnonymousContext_ReturnedObject_ImplementsID2LUserContext() {
			var result = m_appContext.CreateAnonymousUserContext( m_apiHost );

			Assert.IsInstanceOf<ID2LUserContext>( result );
		}

		[Test]
		public void AppContext_CreateAnonymousContext_SaveProperties_BothUserIdAndKey_AreNull() {
			var context = m_appContext.CreateAnonymousUserContext( m_apiHost );
			var props = context.SaveUserContextProperties();

			Assert.IsTrue( props.UserId == null && props.UserKey == null );
		}

		[Test]
		public void AppContext_CreateAnonymousContext_FromSavedProperties_SaveAgain_PropertiesMatch() {
			var originalContext = m_appContext.CreateAnonymousUserContext( m_apiHost );
			var originalProps = originalContext.SaveUserContextProperties();

			var restoredContext = m_appContext.CreateUserContext( originalProps );
			var savedAgainProps = restoredContext.SaveUserContextProperties();

			Assert.IsTrue( originalProps.EqualTo( savedAgainProps ) );
		}
	}
}
