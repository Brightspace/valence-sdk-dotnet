namespace D2L.Extensibility.AuthSdk.UnitTests {
	public static class TestComparisonExtensionMethods {
		public static bool EqualTo( this UserContextProperties expected, UserContextProperties actual ) {
			if( expected == null && actual == null ) {
				return true;
			}
			if( actual == null ) {
				return false;
			}
			return expected.UserId == actual.UserId
			    && expected.UserKey == actual.UserKey
			    && expected.Scheme == actual.Scheme
			    && expected.HostName == actual.HostName
			    && expected.Port == actual.Port;
		}
	}
}
