using System;
using System.Text.RegularExpressions;

namespace D2L.Extensibility.AuthSdk.Impl {

    /// <summary>
    /// Provides a way of extracting the timestamp from a request body if the body says "Timestamp out of range"
    /// </summary>
	internal class TimestampParser {

        /// <summary>
        /// Extracts the timestamp from a request body if the body says "Timestamp out of range"
        /// </summary>
        /// <param name="timestampMessage">The requestr body which may indicated the timestamp is out of range</param>
        /// <param name="result">The timestamp provided by the server</param>
        /// <returns>Whether the skew needs to be changed</returns>
		internal bool TryParseTimestamp( string timestampMessage, out long result ) {
			var regex = new Regex( "Timestamp out of range\\s*(\\d+)", RegexOptions.Multiline );
			Match match = regex.Match( timestampMessage );
			if( match.Success && match.Groups.Count >= 2 ) {
				result = Int64.Parse( match.Groups[ 1 ].Value );
				return true;
			}
			result = 0;
			return false;
		}
	}
}