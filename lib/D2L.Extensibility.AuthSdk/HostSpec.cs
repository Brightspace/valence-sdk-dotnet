using System;

namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// Provides a convenient way of storing and passing server connection information to the library 
    /// </summary>
	public sealed class HostSpec {

        /// <summary>
        /// Constructs a HostSpec with the provided parameters
        /// </summary>
        /// <param name="scheme">The scheme used for communicating with the server (http, https, etc.)</param>
        /// <param name="host">The host name of the server</param>
        /// <param name="port">The port to communicate with the server on</param>
		public HostSpec( string scheme, string host, int port ) {
			m_scheme = scheme;
			m_host = host;
			m_port = port;
		}

        /// <summary>
        /// The scheme used for communicating with the server (http, https, etc.)
        /// </summary>
		public string Scheme { get { return m_scheme; } }

        /// <summary>
        /// The host name of the server
        /// </summary>
		public string Host { get { return m_host; } }

        /// <summary>
        /// The port to communicate with the server on
        /// </summary>
		public int Port { get { return m_port; } }


        /// <summary>
        /// Returns a UriBuilder for the HostSpec
        /// </summary>
        /// <returns>A UriBuilder for the HostSpec</returns>
		public UriBuilder ToUriBuilder() {
			return new UriBuilder( m_scheme, m_host, m_port );
		}

		private readonly string m_scheme;
		private readonly string m_host;
		private readonly int m_port;
	}
}
