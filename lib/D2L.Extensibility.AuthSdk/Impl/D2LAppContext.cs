using System;
using System.Web;

namespace D2L.Extensibility.AuthSdk.Impl {

    /// <summary>
    /// An implementation of ID2LAppContext
    /// </summary>
	internal sealed class D2LAppContext : ID2LAppContext {
		Uri ID2LAppContext.CreateUrlForAuthentication( HostSpec authenticatingHost, Uri landingUri ) {
			var uriBuilder = authenticatingHost.ToUriBuilder();
			uriBuilder.Path = AUTHENTICATION_SERVICE_URI_PATH;
			uriBuilder.Query = BuildAuthenticationUriQueryString( landingUri );
			return uriBuilder.Uri; 
		}

		ID2LUserContext ID2LAppContext.CreateUserContext(
			Uri authenticationCallbackUri, HostSpec apiHost ) {

			var parsingResult = HttpUtility.ParseQueryString( authenticationCallbackUri.Query );
			string userId = parsingResult[USER_ID_CALLBACK_PARAMETER];
			string userKey = parsingResult[USER_KEY_CALLBACK_PARAMETER];
			if( userId == null || userKey == null ) {
				return null;
			}
			return new D2LUserContext( m_timestampProvider, m_appId, m_appKey, userId, userKey, apiHost );
		}

		ID2LUserContext ID2LAppContext.CreateUserContext(
			string userId, string userKey, HostSpec apiHost ) {

			return new D2LUserContext( m_timestampProvider, m_appId, m_appKey, userId, userKey, apiHost );
		}
		
		ID2LUserContext ID2LAppContext.CreateUserContext( UserContextProperties savedProps ) {
			var apiHost = new HostSpec( savedProps.Scheme, savedProps.HostName, savedProps.Port );
			return new D2LUserContext( m_timestampProvider, m_appId, m_appKey, savedProps.UserId, savedProps.UserKey, apiHost );
		}

        ID2LUserContext ID2LAppContext.CreateAnonymousUserContext( HostSpec apiHost ) {
            return new D2LUserContext( m_timestampProvider, m_appId, m_appKey, null, null, apiHost );
        }

		internal D2LAppContext( string appId, string appKey, ITimestampProvider timestampProvider ) {
			m_appId = appId;
			m_appKey = appKey;
			m_timestampProvider = timestampProvider;
		}

        /// <summary>
        /// Constructs a URI to call for authentication given the target URI provided
        /// </summary>
        /// <param name="callbackUri">The target which the D2L server should return to after authenticating</param>
        /// <returns>The URI for the user to authenticate against</returns>
		private string BuildAuthenticationUriQueryString( Uri callbackUri ) {
			string callbackUriString = callbackUri.AbsoluteUri;
			string uriHash = D2LSigner.GetBase64HashString( m_appKey, callbackUriString );
			string result = APP_ID_PARAMETER + "=" + m_appId;
			result += "&" + APP_KEY_PARAMETER + "=" + uriHash;
			result += "&" + CALLBACK_URL_PARAMETER + "=" + HttpUtility.UrlEncode( callbackUriString );
			return result;
		}

	    private readonly ITimestampProvider m_timestampProvider;

		private readonly string m_appId;
		private readonly string m_appKey;

		private const string APP_ID_PARAMETER = "x_a";
		private const string APP_KEY_PARAMETER = "x_b";
		private const string CALLBACK_URL_PARAMETER = "x_target";
		private const string USER_ID_CALLBACK_PARAMETER = "x_a";
		private const string USER_KEY_CALLBACK_PARAMETER = "x_b";
		private const string AUTHENTICATION_SERVICE_URI_PATH = "/d2l/auth/api/token";
	}
}
