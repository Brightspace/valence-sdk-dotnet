using D2L.Extensibility.AuthSdk.Impl;

namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// A class for providing an instance of ID2LAppContext
    /// </summary>
	public class D2LAppContextFactory {

        /// <summary>
        /// Default constructor for D2LAppContextFactory
        /// </summary>
		public D2LAppContextFactory() {
			DependencyContainer.BootstrapStructureMap();
		}

        /// <summary>
        /// Creates an instance of ID2LAppContext with the specified app ID and app key
        /// </summary>
        /// <param name="appId">The D2L app ID</param>
        /// <param name="appKey">The D2L app key</param>
        /// <returns>An instance of ID2LAppContext</returns>
		public ID2LAppContext Create( string appId, string appKey ) {
			return new D2LAppContext( appId, appKey );
		}
	}
}
