namespace D2L.Extensibility.AuthSdk.IntegrationTests.Providers {
    
    internal static class RouteProvider {

        private const string GET_ORGANIZATION_INFO_ROUTE = "/d2l/api/lp/1.0/organization/info";

        internal static string OrganizationInfoRoute { get { return GET_ORGANIZATION_INFO_ROUTE; } }
    }
}
