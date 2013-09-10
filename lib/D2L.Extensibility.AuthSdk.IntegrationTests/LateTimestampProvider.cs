using System;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {
	internal sealed class LateTimestampProvider : ITimestampProvider {
		public LateTimestampProvider( long lagSeconds ) {
			m_lagSeconds = lagSeconds;
		}

		long ITimestampProvider.GetCurrentTimestampInMilliseconds() {
			var now = DateTime.UtcNow;
			var jan1_1970 = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			TimeSpan span = now.Subtract( jan1_1970 );
			return (long)span.TotalMilliseconds - m_lagSeconds * 1000;
		}

		private readonly long m_lagSeconds;
	}
}
