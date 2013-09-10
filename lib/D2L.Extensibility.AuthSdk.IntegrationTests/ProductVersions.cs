using System;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {
	[Serializable]
	public sealed class ProductVersions {
		public string LatestVersion { get; set; }
		public string ProductCode { get; set; }
		public string[] SupportedVersions { get; set; }
	}
}
