using System;
using System.Security.Cryptography;
using System.Text;

namespace D2L.Extensibility.AuthSdk.Impl {

    /// <summary>
    /// A convenience class to help with the signature generation used in the D2L authentication system
    /// </summary>
	internal static class D2LSigner {

        /// <summary>
        /// Provides the D2L custom encoded version of hmacSha256 hash of the data provided using the key provided
        /// </summary>
        /// <param name="key">The key to use to calculate the hash</param>
        /// <param name="data">The data to use to calculate the hash</param>
        /// <returns></returns>
		internal static string GetBase64HashString( string key, string data ) {
			byte[] keyBytes = GetBytes( key );
			byte[] dataBytes = GetBytes( data );
			byte[] hash = ComputeHash( keyBytes, dataBytes );
			string result = CustomBase64Encode( hash );
			return result;
		}

        /// <summary>
        /// Converts the hash to a string and replaces characters where appropriate
        /// </summary>
        /// <param name="hash">The hash to convert</param>
        /// <returns>The D2L custom base 64 encoded result string</returns>
		private static string CustomBase64Encode( byte[] hash ) {
			string base64String = Convert.ToBase64String( hash );
			string result = base64String.Replace( "=", "" );
			result = result.Replace( "+", "-" );
			result = result.Replace( "/", "_" );
			return result;
		}

        /// <summary>
        /// Provides the byte value of the given String
        /// </summary>
        /// <param name="key">The String to return the bytes of</param>
        /// <returns>The bytes representing the given string</returns>
		private static byte[] GetBytes( string key ) {
			return Encoding.UTF8.GetBytes( key );
		}

        /// <summary>
        /// Computes the hmacSha256 hash of the data using the key given
        /// </summary>
        /// <param name="keyBytes">The key to use to calculate the hash</param>
        /// <param name="dataBytes">The data to use to calculate the hash</param>
        /// <returns>The hmacSha256 hash of the data using the key</returns>
		private static byte[] ComputeHash( byte[] keyBytes, byte[] dataBytes ) {
			byte[] hash;
			using( var hmacSha256 = new HMACSHA256( keyBytes ) ) {
				hash = hmacSha256.ComputeHash( dataBytes );
			}
			return hash;
		}
	}
}
