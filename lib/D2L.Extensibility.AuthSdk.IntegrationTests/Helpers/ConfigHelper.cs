namespace D2L.Extensibility.AuthSdk.IntegrationTests.Helpers {
	internal static class ConfigHelper {

		internal static string AppId { get; private set; }
		internal static string AppKey { get; private set; }
		internal static string UserId { get; private set; }
		internal static string UserKey { get; private set; }
		internal static string Scheme { get; private set; }
		internal static string Host { get; private set; }
		internal static int Port { get; private set; }

		static ConfigHelper() {
			AppId = "G9nUpvbZQyiPrk3um2YAkQ";
			AppKey = "ybZu7fm_JKJTFwKEHfoZ7Q";
			UserId = "SomeUserId";
			UserKey = "SomeUserKey";
			Scheme = "https";
			Host = "devcop.brightspacedemo.com";
			Port = 443;
		}
	}
}
