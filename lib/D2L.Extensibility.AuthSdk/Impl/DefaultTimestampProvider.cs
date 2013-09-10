using System;

namespace D2L.Extensibility.AuthSdk.Impl {

    /// <summary>
    /// Provides an instance of ITimestampProvider
    /// </summary>
	internal sealed class DefaultTimestampProvider : ITimestampProvider {
		long ITimestampProvider.GetCurrentTimestampInMilliseconds() {
			var now = DateTime.UtcNow;
			var jan1_1970 = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			TimeSpan span = now.Subtract( jan1_1970 );
			return (long)span.TotalMilliseconds;
		}
	}
}
