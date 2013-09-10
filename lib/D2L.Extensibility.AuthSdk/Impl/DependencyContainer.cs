using StructureMap;

namespace D2L.Extensibility.AuthSdk.Impl {

    /// <summary>
    /// Handles dependency injection
    /// </summary>
	public static class DependencyContainer {

        /// <summary>
        /// Injects all dependencies
        /// </summary>
		static DependencyContainer() {
			BootstrapStructureMap();
		}

        /// <summary>
        /// Injects depencies for factory use
        /// </summary>
		public static void BootstrapStructureMap() {
			ObjectFactory.Initialize( x => x.For<ITimestampProvider>().Use<DefaultTimestampProvider>() );
		}
	}
}
