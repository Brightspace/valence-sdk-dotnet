using System;

namespace D2L.Extensibility.AuthSdk.IntegrationTests {
	[Serializable]
	public sealed class Organization {
		public long Identifier { get; set; }
		public string Name { get; set; }
	}
}
