using System;
using System.IO;
using System.Net;
using System.Text;

namespace D2L.Extensibility.AuthSdk.IntegrationTests.Helpers {

    internal static class StringHelper {

        internal static string ReadResponseContents( HttpWebResponse response ) {
            using ( var stream = response.GetResponseStream() ) {
                using ( var reader = new StreamReader( stream, Encoding.UTF8 ) ) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
