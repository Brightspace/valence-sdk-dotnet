using System;
using System.Configuration;

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
            AppId = ConfigurationManager.AppSettings["appId"];
            AppKey = ConfigurationManager.AppSettings["appKey"];
            UserId = ConfigurationManager.AppSettings["userId"];
            UserKey = ConfigurationManager.AppSettings["userKey"];
            Scheme = ConfigurationManager.AppSettings["scheme"];
            Host = ConfigurationManager.AppSettings["host"];
            Port = Int32.Parse( ConfigurationManager.AppSettings["port"] );
        }
    }
}
