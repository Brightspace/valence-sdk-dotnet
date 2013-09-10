using System;
using System.Net;

using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	[TestFixture]
	public sealed class ResultInterpretationTests {
		[SetUp]
		public void SetUpTestUserContext() {
			m_userContext = TestUtils.CreateTestUserContext();
		}

		[Test]
		public void WhenStatusCodeIs403_And_ResponseBodyIs_Invalid_token_Returns_InvalidSig() {
			var exception = new FullD2LWebExceptionStub( HttpStatusCode.Forbidden, "Invalid token" );

			var interpretation = m_userContext.InterpretResult( exception );
			
			Assert.AreEqual( RequestResult.RESULT_INVALID_SIG, interpretation );
		}

		[Test]
		public void WhenStatusCodeIs403_And_CaseOfInvalidToken_DoesNotAffectResult() {
			var exception = new FullD2LWebExceptionStub( HttpStatusCode.Forbidden, "Invalid Token" );

			var interpretation = m_userContext.InterpretResult( exception );

			Assert.AreEqual( RequestResult.RESULT_INVALID_SIG, interpretation );
		}

		[Test]
		public void WhenStatusCodeIs400_Returns_BadRequest() {
			var exception = new CodeOnlyD2LWebExceptionMock( HttpStatusCode.BadRequest );

			var interpretation = m_userContext.InterpretResult( exception );

			Assert.AreEqual( RequestResult.BAD_REQUEST, interpretation );
		}
		
		[Test]
		public void WhenStatusCodeIs404_Returns_NotFound() {
			var exception = new CodeOnlyD2LWebExceptionMock( HttpStatusCode.NotFound );

			var interpretation = m_userContext.InterpretResult( exception );

			Assert.AreEqual( RequestResult.NOT_FOUND, interpretation );
		}

		[Test]
		public void WhenStatusCodeIs500_Returns_InternalServerError() {
			var exception = new CodeOnlyD2LWebExceptionMock( HttpStatusCode.InternalServerError );

			var interpretation = m_userContext.InterpretResult( exception );

			Assert.AreEqual( RequestResult.INTERNAL_SERVER_ERROR, interpretation );
		}

		[Test]
		public void WhenStatusCodeIs_SomethingElseEg502_Returns_Unknown() {
			var exception = new CodeOnlyD2LWebExceptionMock( HttpStatusCode.BadGateway);

			var interpretation = m_userContext.InterpretResult( exception );

			Assert.AreEqual( RequestResult.RESULT_UNKNOWN, interpretation );
		}
		
		[Test]
		public void WhenStatusCodeIs403_And_ResponseBodyIsEmpty_Returns_NoPermission() {
			var exception = new FullD2LWebExceptionStub( HttpStatusCode.Forbidden, "" );

			var interpretation = m_userContext.InterpretResult( exception );

			Assert.AreEqual( RequestResult.RESULT_NO_PERMISSION, interpretation );
		}

		[Test]
		public void OpContext_ServerSkewMillis_get_Returns_SameAsSetValue() {
			const long expectedSkew = 14438L;
			var opContext = TestUtils.CreateTestUserContext();
			opContext.ServerSkewMillis = expectedSkew;

			long actualSkew = opContext.ServerSkewMillis;
			Assert.AreEqual( expectedSkew, actualSkew );
		}

		[Test]
		public void UserContext_InterpretResult_Upon403_ChangesServerSkewProperty_ByTimestampDifference() {
			var appContext = TestUtils.CreateAppContextUnderTest();
			const long clientTimeSeconds = 1319000000L;
			TestUtils.SetUpTimestampProviderStub( clientTimeSeconds * 1000 );
			var apiHost = TestUtils.CreateTestHost();
			var userContext = appContext.CreateAnonymousUserContext( apiHost );
			const long serverAheadBy = 907L;
			string responseBody = String.Format( "Timestamp out of range\r\n{0}",
												 clientTimeSeconds + serverAheadBy );
			var exception = new FullD2LWebExceptionStub( HttpStatusCode.Forbidden, responseBody );

			userContext.InterpretResult( exception );

			Assert.AreEqual( serverAheadBy * 1000, userContext.ServerSkewMillis );
		}

		[Test]
		public void UserContext_InterpretResult_Upon403_AndResponseTimestampIsInvalid_ServerSkewPropertyDoesNotChange() {
			var userContext = TestUtils.CreateTestUserContext();
			const long expectedSkew = 874000L;
			userContext.ServerSkewMillis = expectedSkew;
			var exception = new FullD2LWebExceptionStub(
				HttpStatusCode.Forbidden, "Timestamp out of range\r\n" );

			userContext.InterpretResult( exception );
			Assert.AreEqual( expectedSkew, userContext.ServerSkewMillis );
		}

		private ID2LUserContext m_userContext;
	}
}
