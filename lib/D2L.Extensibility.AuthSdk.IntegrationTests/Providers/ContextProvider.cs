using System;
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

        #region User context methods

        internal static ID2LUserContext AnonUserContext() {
            return m_appContext.CreateAnonymousUserContext( CreateHostSpec() );
        }

        internal static ID2LUserContext AnonUserContext( HostSpec host ) {
            return m_appContext.CreateAnonymousUserContext( host );
        }

        internal static ID2LUserContext BadUserContext() {
            return m_appContext.CreateUserContext( "foo", "bar", CreateHostSpec() );
        }

        internal static ID2LUserContext UserContext() {
            return m_appContext.CreateUserContext( ConfigHelper.UserId, ConfigHelper.UserKey, CreateHostSpec() );
        }

        internal static ID2LUserContext UserContext( ID2LAppContext appContext ) {
            return appContext.CreateUserContext( ConfigHelper.UserId, ConfigHelper.UserKey, CreateHostSpec() );
        }
        #endregion

        /// <summary>
        /// Created an authenticated URL using the default application context.
        /// </summary>
        /// <param name="url">The landing URL.</param>
        /// <returns>A complete URL with the default application information and the specified landing URL.</returns>
        internal static Uri AuthenticatedUrl( Uri url ) {
            return m_appContext.CreateUrlForAuthentication( CreateHostSpec(), url );
        }

        private static HostSpec CreateHostSpec() {
            return new HostSpec( ConfigHelper.Scheme, ConfigHelper.Host, ConfigHelper.Port );
        }


    }
}
