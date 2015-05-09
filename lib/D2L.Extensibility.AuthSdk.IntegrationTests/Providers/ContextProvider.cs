using D2L.Extensibility.AuthSdk;
using D2L.Extensibility.AuthSdk.IntegrationTests.Helpers;

namespace D2L.Extensibility.AuthSdk.IntegrationTests.Providers {

    internal static class ContextProvider {

        private static D2LAppContextFactory m_appContextFactory;
        private static ID2LAppContext m_appContext;

        static ContextProvider() {
            m_appContextFactory = new D2LAppContextFactory();
            m_appContext = m_appContextFactory.Create( ConfigHelper.AppId, ConfigHelper.AppKey );
        }

        internal static ID2LUserContext AnonUserContext() {
            return m_appContext.CreateAnonymousUserContext( CreateHostSpec() );
        }

        internal static ID2LUserContext BadUserContext() {
            return m_appContext.CreateUserContext( "foo", "bar", CreateHostSpec() );
        }

        internal static ID2LUserContext UserContext() {
            return m_appContext.CreateUserContext( ConfigHelper.UserId, ConfigHelper.UserKey, CreateHostSpec() );
        }

        private static HostSpec CreateHostSpec() {
            return new HostSpec( ConfigHelper.Scheme, ConfigHelper.Host, ConfigHelper.Port );
        }


    }
}
