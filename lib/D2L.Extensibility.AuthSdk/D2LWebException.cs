using System.IO;
using System.Net;
using System.Text;

namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// Provides a wrapper around a WebException to simplify getting the result from it
    /// </summary>
	public class D2LWebException {

        /// <summary>
        /// The HttpStatusCode of the Exception
        /// </summary>
		public virtual HttpStatusCode StatusCode { get { return m_statusCode; } }

        /// <summary>
        /// The message body retrieved from the Exception
        /// </summary>
		public virtual string ResponseBody { get { return m_responseBody; } }

        /// <summary>
        /// Constructs a D2LWebException and if possible reads the HttpStatusCode and message body of the response from the Exception
        /// </summary>
        /// <param name="ex">The WebException to attempt to read the HttpStatusCode and message body from</param>
		public D2LWebException( WebException ex ) {
			if( ex != null ) {
				var castResponse = ex.Response as HttpWebResponse;
				if( castResponse != null ) {
					m_statusCode = castResponse.StatusCode;
					using( var stream = castResponse.GetResponseStream() ) {
						if( stream != null ) {
							using( var reader = new StreamReader( stream, Encoding.UTF8 ) ) {
								m_responseBody = reader.ReadToEnd();
							}
						}
					}
				}
			}
		}

		private readonly HttpStatusCode m_statusCode = HttpStatusCode.NotImplemented;
		private readonly string m_responseBody = "";
	}
}
