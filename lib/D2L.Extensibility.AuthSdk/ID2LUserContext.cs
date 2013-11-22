using System;
using System.Collections.Generic;

namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// Provides methods to communicate with a server using user credentials and provides some help with handling results and dealing with invalid timestamps
    /// </summary>
    public interface ID2LUserContext {

        /// <summary>
        /// The expected time between when the client sends a request and the server receives it
        /// </summary>
        long ServerSkewMillis { get; set; }

        /// <summary>
        /// Creates a URI to access the D2L server with the given path and http method
        /// </summary>
        /// <param name="path">The path on the server to call</param>
        /// <param name="httpMethod">The http method to make the call with</param>
        /// <returns>The resultant URI</returns>
        Uri CreateAuthenticatedUri( string path, string httpMethod );

        /// <summary>
        /// Creates a URI to access the D2L server with the given full URL and http method
        /// </summary>
        /// <param name="fullUrl">The fullUrl to the server with the route to call.</param>
        /// <param name="httpMethod">The http method to make the call with</param>
        /// <returns>The resultant URI</returns>
        Uri CreateAuthenticateUri( Uri fullUrl, string httpMethod );

        /// <summary>
        /// Creates the tokens to access the D2L server with the given path and http method
        /// </summary>
        /// <param name="path">The path on the server to call</param>
        /// <param name="httpMethod">The http method to make the call with</param>
        /// <returns>The resultant URI</returns>
        IEnumerable<Tuple<string, string>> CreateAuthenticatedTokens( string path, string httpMethod );

        /// <summary>
        /// Creates the tokens to access the D2L server with the given full URL and http method
        /// </summary>
        /// <param name="fullUrl">The fullUrl to the server with the route to call.</param>
        /// <param name="httpMethod">The http method to make the call with</param>
        /// <returns>The resultant URI</returns>
        IEnumerable<Tuple<string, string>> CreateAuthenticatedTokens( Uri fullUrl, string httpMethod );

        /// <summary>
        /// This utility method can be used to process results from the server, it interprets http result code and 
        /// optional body messages on error (e.g. distinguishing timestamp skew from bad signature). In addition, 
        /// in the special case of timestamp skew this method will update the internal skew so that the next request should avoid this error. 
        /// </summary>
        /// <param name="exceptionWrapper">The wrapper for the WebException to interpret</param>
        /// <returns>The RequestResult from the request</returns>
        RequestResult InterpretResult( D2LWebException exceptionWrapper );

        /// <summary>
        /// Returns a serializable version of the properties used by this user context
        /// </summary>
        /// <returns>A serializable version of the properties used by this user context</returns>
        UserContextProperties SaveUserContextProperties();
    }
}
