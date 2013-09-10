using NUnit.Framework;
using StructureMap;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	public class FactoryTests {
		[Test]
		public void AppContextFactory_Create_ReturnedObject_Implements_ID2LAppContext() {
			var factory = new D2LAppContextFactory();

			var context = factory.Create( "foo", "bar" );

			Assert.IsInstanceOf<ID2LAppContext>( context );
		}

		[Test]
		public void StructureMap_UponAppContextFactoryConstruction_HasWiringFor_ITimestampProvider() {
			new D2LAppContextFactory();

			var timestampProvider = ObjectFactory.GetInstance<ITimestampProvider>();
			Assert.IsNotNull( timestampProvider );
		}
	}
}
