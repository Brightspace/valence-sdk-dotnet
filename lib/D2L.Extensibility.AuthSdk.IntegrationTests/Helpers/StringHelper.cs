using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace D2L.Extensibility.AuthSdk.IntegrationTests.Helpers {

    internal static class StringHelper {

        internal static T DeserializeResponseContents<T>( HttpWebResponse response ) where T : class {
            string contents = StringHelper.ReadResponseContents( response );
            var serializer = new JavaScriptSerializer();
            var resource = serializer.Deserialize<T>( contents );
            return resource;
        }

        internal static string ReadResponseContents( HttpWebResponse response ) {
            using ( var stream = response.GetResponseStream() ) {
                using ( var reader = new StreamReader( stream, Encoding.UTF8 ) ) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
