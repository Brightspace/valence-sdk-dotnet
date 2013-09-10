using System;

namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// Stores parameters for a user context in a format that can be easily serialized for caching purposes 
    /// </summary>
	[Serializable]
	public sealed class UserContextProperties {

        /// <summary>
        /// The D2L user ID (aka token ID)
        /// </summary>
		public string UserId { get; set; }

        /// <summary>
        /// The D2L user key (aka token key)
        /// </summary>
		public string UserKey { get; set; }

        /// <summary>
        /// The scheme used for communicating with the server (http, https, etc.)
        /// </summary>
		public string Scheme { get; set; }

        /// <summary>
        /// The host name of the server to make API calls to
        /// </summary>
		public string HostName { get; set; }

        /// <summary>
        /// The port to communicate with the API server on
        /// </summary>
		public int Port { get; set; }
	}
}
