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
        // Provision this static helper with the ID-Key App Key, App ID, and Brightspace domain
			AppId = "VALENCE_APP_ID_HERE";
			AppKey = "VALENCE_APP_KEY_HERE";
			UserId = "SomeUserId";
			UserKey = "SomeUserKey";
			Scheme = "https";
			Host = "BRIGHTSPACE_DOMAIN_HERE";
			Port = 443;
		}
	}
}
