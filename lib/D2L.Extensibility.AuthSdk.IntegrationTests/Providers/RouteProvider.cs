namespace D2L.Extensibility.AuthSdk.IntegrationTests.Providers {
    
    internal static class RouteProvider {

        private const string GET_ORGANIZATION_INFO_ROUTE = "/d2l/api/lp/1.0/organization/info";
        private const string GET_VERSIONS_ROUTE = "/d2l/api/versions/";

        internal static string OrganizationInfoRoute { get { return GET_ORGANIZATION_INFO_ROUTE; } }
        internal static string VersionsRoute { get { return GET_VERSIONS_ROUTE; } }
    }
}
