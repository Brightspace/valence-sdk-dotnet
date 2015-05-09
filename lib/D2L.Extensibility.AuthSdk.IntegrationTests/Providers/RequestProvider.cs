using System;
using System.Net;

namespace D2L.Extensibility.AuthSdk.IntegrationTests.Providers {

    internal static class RequestProvider {

        internal static HttpWebRequest PrepareApiRequest( ID2LUserContext userContext, string route ) {
            Uri apiUri = userContext.CreateAuthenticatedUri( route, "GET" );
            return CreateRequest( apiUri );
        }

        internal static HttpWebRequest CreateRequest( Uri uri ) {
            var request = (HttpWebRequest)WebRequest.Create( uri );
            request.Method = "GET";
            request.Accept = "*/*";

            //	keep the timeout at least 10 seconds,
            //	otherwise some important errors turn into "operation timed-out" errors
            request.Timeout = 10 * 1000;
            return request;
        }

    }
}
