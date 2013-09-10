using System.Net;

using NUnit.Framework;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	internal sealed class CodeOnlyD2LWebExceptionMock : D2LWebException {
		public override string ResponseBody {
			get {
				throw new AssertionException( "Zero calls to this property getter were expected" );
			}
		}

		public override HttpStatusCode StatusCode {
			get { return m_statusCode; }
		}

		internal CodeOnlyD2LWebExceptionMock( HttpStatusCode statusCode ) : base( null ) {
			m_statusCode = statusCode;
		}

		private readonly HttpStatusCode m_statusCode;
	}
}
