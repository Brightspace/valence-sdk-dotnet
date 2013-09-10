using System.Net;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	internal sealed class FullD2LWebExceptionStub : D2LWebException {
		public override HttpStatusCode StatusCode {
			get { return m_statusCode; }
		}

		public override string ResponseBody {
			get { return m_responseBody; }
		}

		internal FullD2LWebExceptionStub( HttpStatusCode statusCode, string responseBody )
			: base( null ) {

			m_statusCode = statusCode;
			m_responseBody = responseBody;
		}

		private readonly HttpStatusCode m_statusCode;
		private readonly string m_responseBody;
	}
}
