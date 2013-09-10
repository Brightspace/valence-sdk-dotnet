using System;
using D2L.Extensibility.AuthSdk.Impl;
using StructureMap;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	internal sealed class TestUtils {
		internal static ID2LAppContext CreateAppContextUnderTest() {
			var factory = new D2LAppContextFactory();
			return factory.Create( TestConstants.APP_ID, TestConstants.APP_KEY );
		}

		internal static Uri CreateTestAuthenticationCallbackUri( string userId, string userKey ) {
			string uriString = TestConstants.API_URL + "?";
			if( !String.IsNullOrEmpty( userId ) ) {
				uriString += "x_a=" + userId;
			}
			if( !String.IsNullOrEmpty( userKey ) ) {
				uriString += "&x_b=" + userKey;
			}
			return new Uri( uriString );
		}

		internal static HostSpec CreateTestHost() {
			return new HostSpec( TestConstants.SCHEME, TestConstants.HOST_NAME, TestConstants.PORT );
		}

		internal static ID2LUserContext CreateTestUserContext() {
			var appContext = CreateAppContextUnderTest();
			var apiHost = CreateTestHost();
			return appContext.CreateUserContext( TestConstants.USER_ID, TestConstants.USER_KEY, apiHost );
		}

		internal static string GetUriQueryParameter( Uri uri, string name ) {
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

		internal static string CalculateParameterExpectation(
			string key, string httpMethod, string apiPath, long timestamp ) {

			string unsignedResult = String.Format( "{0}&{1}&{2}", httpMethod, apiPath, timestamp );
			string signedResult = D2LSigner.GetBase64HashString( key, unsignedResult );
			return signedResult;
		}
		
		internal static void SetUpTimestampProviderStub( long milliseconds ) {
			ObjectFactory.Initialize( x => {
				x.For<ITimestampProvider>()
				.Use<TimestampProviderStub>()
				.WithCtorArg( "milliseconds" )
				.EqualTo( milliseconds );
			} );
		}
	}
}
