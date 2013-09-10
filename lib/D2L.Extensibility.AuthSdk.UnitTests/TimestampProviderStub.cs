namespace D2L.Extensibility.AuthSdk.UnitTests {
	public sealed class TimestampProviderStub : ITimestampProvider {
		long ITimestampProvider.GetCurrentTimestampInMilliseconds() {
			return m_milliseconds;
		}

		public TimestampProviderStub( long milliseconds ) {
			m_milliseconds = milliseconds;
		}

		private readonly long m_milliseconds;
	}
}
