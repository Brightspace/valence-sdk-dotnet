using System;
using System.Net;

namespace D2L.Extensibility.AuthSdk.Impl {

    /// <summary>
    /// An implementation of ID2LUserContext
    /// </summary>
	internal sealed class D2LUserContext : ID2LUserContext {

		long ID2LUserContext.ServerSkewMillis {
			get { return m_serverSkewMillis; }
			set { m_serverSkewMillis = value; }
		}

		Uri ID2LUserContext.CreateAuthenticatedUri(string path, string httpMethod)
        {
            Uri initialUri = new Uri(m_apiHost.Scheme + "://" + m_apiHost.Host + path);

			string queryString = GetQueryString( initialUri.AbsolutePath, initialUri.Query, httpMethod );
        	var uriBuilder = m_apiHost.ToUriBuilder();
            uriBuilder.Path = initialUri.AbsolutePath;
        	uriBuilder.Query = queryString;
			return uriBuilder.Uri;
		}

        UserContextProperties ID2LUserContext.SaveUserContextProperties()
        {
			return new UserContextProperties() {
				UserId = m_userId,
				UserKey = m_userKey,
				Scheme = m_apiHost.Scheme,
				HostName = m_apiHost.Host,
				Port = m_apiHost.Port
			};
		}

        /// <summary>
        /// Adjusts the server skew if necessary based on the body of the response
        /// </summary>
        /// <param name="responseBody">The body of the response</param>
        /// <returns>Whether the timestamp was changed or not</returns>
        private bool TryCalculateServerSkewFromResponse( string responseBody ) {
			var timestampParser = new TimestampParser();
			long serverTimestampSeconds;
			if( timestampParser.TryParseTimestamp( responseBody, out serverTimestampSeconds ) ) {
				long clientTimestampMilliseconds = m_timestampProvider.GetCurrentTimestampInMilliseconds();
				m_serverSkewMillis = serverTimestampSeconds * 1000 - clientTimestampMilliseconds;
                return true;
			}
            return false;
		}

		RequestResult ID2LUserContext.InterpretResult( D2LWebException exceptionWrapper ) {
			switch( exceptionWrapper.StatusCode ) {
				case HttpStatusCode.Forbidden:
					return InterpretStatusCodeForbidden( exceptionWrapper.ResponseBody );
				case HttpStatusCode.BadRequest:
					return RequestResult.BAD_REQUEST;
				case HttpStatusCode.NotFound:
					return RequestResult.NOT_FOUND;
				case HttpStatusCode.InternalServerError:
					return RequestResult.INTERNAL_SERVER_ERROR;
				default:
					return RequestResult.RESULT_UNKNOWN;
			}
		}

        /// <summary>
        /// Helper method fpor interpreting results when a 403 is received.
        /// </summary>
        /// <param name="responseBody"></param>
        /// <returns></returns>
		private RequestResult InterpretStatusCodeForbidden( string responseBody ) {
			if( TryCalculateServerSkewFromResponse( responseBody ) ) {
				return RequestResult.RESULT_INVALID_TIMESTAMP;
			}
			if( responseBody.Equals( "Invalid token",StringComparison.InvariantCultureIgnoreCase )) {
				return RequestResult.RESULT_INVALID_SIG;
			}
			return RequestResult.RESULT_NO_PERMISSION;
		}
		
        /// <summary>
        /// Constructs a D2LUserContext with the parameters provided
        /// </summary>
        /// <param name="timestampProvider">The system timestamp provider</param>
        /// <param name="appId">The D2L app ID</param>
        /// <param name="appKey">The D2L app key</param>
        /// <param name="userId">The D2L user ID to be used</param>
        /// <param name="userKey">The D2L user key to be used</param>
        /// <param name="apiHost">The host information of the server to make API calls to</param>
		internal D2LUserContext( ITimestampProvider timestampProvider, string appId, string appKey, string userId, string userKey, HostSpec apiHost ) {
			m_appId = appId;
			m_appKey = appKey;
			m_userId = userId;
			m_userKey = userKey;
			m_apiHost = apiHost;
	        m_timestampProvider = timestampProvider;
        }
		
        /// <summary>
        /// Constructs a query string with the necessary auth parameters to connect to a D2L API server
        /// </summary>
        /// <param name="path">The absolute path for the request (ie /d2l/api/versions/)</param>
        /// <param name="httpMethod">The http method used (ie GET,POST) </param>
        /// <returns>A query string with the necessary auth parameters to connect to a D2L API server</returns>
		private string GetQueryString( string path, string query, string httpMethod ) {
			long adjustedTimestampSeconds = GetAdjustedTimestampInSeconds();
			string signature = FormatSignature( path, httpMethod, adjustedTimestampSeconds );
			string queryString = BuildAuthenticatedUriQueryString( signature, adjustedTimestampSeconds );
            if (query != "")
            {
                queryString += "&" + query.Substring(1);
            }
			return queryString;
		}

        /// <summary>
        /// Returns the timestamp in milliseconds adjusting for the calculated skew
        /// </summary>
        /// <returns>The timestamp in milliseconds adjusting for the calculated skew</returns>
		private long GetAdjustedTimestampInSeconds() {
			long timestampMilliseconds = m_timestampProvider.GetCurrentTimestampInMilliseconds();
			long adjustedTimestampSeconds = ( timestampMilliseconds + m_serverSkewMillis ) / 1000;
			return adjustedTimestampSeconds;
		}

        /// <summary>
        /// Constructs an authenticated query string with the necessary auth parameters to connect to a D2L API server
        /// </summary>
        /// <param name="signature">The signature used to calculate the user key and app key</param>
        /// <param name="timestamp">The timestamp to use for the request</param>
        /// <returns>An authenticated query string with the necessary auth parameters to connect to a D2L API server</returns>
		private string BuildAuthenticatedUriQueryString( string signature, long timestamp ) {
			string queryString = APP_ID_PARAMETER + "=" + m_appId;
            if (m_userId != null)
            {
                queryString += "&" + USER_ID_PARAMETER + "=" + m_userId;
                queryString += "&" + SIGNATURE_BY_USER_KEY_PARAMETER;
                queryString += "=" + D2LSigner.GetBase64HashString(m_userKey, signature);
            }
			queryString += "&" + SIGNATURE_BY_APP_KEY_PARAMETER;
			queryString += "=" + D2LSigner.GetBase64HashString( m_appKey, signature );
			queryString += "&" + TIMESTAMP_PARAMETER + "=" + timestamp;
			return queryString;
		}


        /// <summary>
        /// Formats a signature to the format required by the D2L API servers
        /// </summary>
        /// <param name="path">The absolute path for the request (ie /d2l/api/versions/)</param>
        /// <param name="httpMethod">The http method used (ie GET,POST)</param>
        /// <param name="timestampSeconds">The timestamp to use, in seconds, for the request</param>
        /// <returns></returns>
		private static string FormatSignature( string path, string httpMethod, long timestampSeconds ) {
			// Note: We UrlDecode the path to handle the (rare) case that the path needs to be urlencoded. The LMS checks
			// the signature of the decoded path so we must sign it appropriately.
			return String.Format( "{0}&{1}&{2}",
					httpMethod.ToUpperInvariant(), System.Web.HttpUtility.UrlDecode(path).ToLowerInvariant(), timestampSeconds );
		}
		
		private readonly string m_appId;
		private readonly string m_appKey;
		private readonly string m_userId;
		private readonly string m_userKey;
		private readonly HostSpec m_apiHost;
		private readonly ITimestampProvider m_timestampProvider;

		private long m_serverSkewMillis;

		private const string APP_ID_PARAMETER = "x_a";
		private const string USER_ID_PARAMETER = "x_b";
		private const string SIGNATURE_BY_APP_KEY_PARAMETER = "x_c";
		private const string SIGNATURE_BY_USER_KEY_PARAMETER = "x_d";
		private const string TIMESTAMP_PARAMETER = "x_t";
	}
}
