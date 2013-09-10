using System;

namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// Stores the state of the application (app ID and app key) and provides methods for authentication 
    /// and for creating instances of ID2LUserContext with the appropriate information.
    /// </summary>
	public interface ID2LAppContext {

        /// <summary>
        /// Provides the URL on the D2L server for users to authenticate against
        /// </summary>
        /// <param name="authenticatingHost"></param>
        /// <param name="landingUri"></param>
        /// <returns></returns>
		Uri CreateUrlForAuthentication( HostSpec authenticatingHost, Uri landingUri );

        /// <summary>
        /// Creates an instance of ID2LUserContext with user parameters specified in the URI
        /// </summary>
        /// <param name="authenticationCallbackUri">The URI containing the user id and user key returned by the server</param>
        /// <param name="apiHost">The host information of the server to make API calls to</param>
        /// <returns>An instance of ID2LUserContext with user parameters specified in the URI</returns>
		ID2LUserContext CreateUserContext( Uri authenticationCallbackUri, HostSpec apiHost );

        /// <summary>
        /// Creates an instance of ID2LUserContext with user parameters provided
        /// </summary>
        /// <param name="userId">The D2L user ID to be used</param>
        /// <param name="userKey">The D2L user key to be used</param>
        /// <param name="apiHost">The host information of the server to make API calls to</param>
        /// <returns>An instance of ID2LUserContext with user parameters provided</returns>
		ID2LUserContext CreateUserContext( string userId, string userKey, HostSpec apiHost );

        /// <summary>
        /// Creates an instance of ID2LUserContext from the given properties
        /// </summary>
        /// <param name="savedProps">Properties to load</param>
        /// <returns>An instance of ID2LUserContext from the given properties</returns>
		ID2LUserContext CreateUserContext( UserContextProperties savedProps );

        /// <summary>
        /// Creates an instance of ID2LUserContext without user credentials
        /// </summary>
        /// <param name="apiHost">The host information of the server to make API calls to</param>
        /// <returns>An instance of ID2LUserContext without user credentials</returns>
		ID2LUserContext CreateAnonymousUserContext( HostSpec apiHost );

        

       

        
	}
}
