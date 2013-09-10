using NUnit.Framework;

using D2L.Extensibility.AuthSdk.Impl;

namespace D2L.Extensibility.AuthSdk.UnitTests {
	[TestFixture]
	public sealed class SigningParsingTests {
		[Test]
		public void D2LSigner_GetBase64HashString_RemovesTrailingEqualSigns() {
			string hash = D2LSigner.GetBase64HashString( TEST_KEY, TEST_DATA );
			Assert.AreEqual( TEST_HASH, hash );
		}

		[Test]
		public void D2LSigner_GetBase64HashString_ReplacesPlusesWithDashes() {
			string hash = D2LSigner.GetBase64HashString( TEST_KEY, TEST_DATA_2 );
			Assert.AreEqual( TEST_HASH_2, hash );
		}

		[Test]
		public void D2LSigner_GetBase64HashString_ReplacesSlashesWithUnderscores() {
			string hash = D2LSigner.GetBase64HashString( TEST_KEY, TEST_DATA_3 );
			Assert.AreEqual( TEST_HASH_3, hash );
		}

		[Test]
		public void DefaultTimestampProvider_GetTimestamp_ReturnsValueInRange() {
			ITimestampProvider provider = new DefaultTimestampProvider();
			long timestamp = provider.GetCurrentTimestampInMilliseconds();
			Assert.That( ref timestamp,
				Is.AtMost( IF_MORE_BASE_YEAR_MAY_BE_WRONG ).And.AtLeast( IF_LESS_UNITS_ARE_WRONG ) );
		}

		[Test]
		public void TimestampParser_GivenTimestampOutOfRangeMessage_ParsesTimestampOutOfIt() {
			var parser = new TimestampParser();
			long timestamp;
			parser.TryParseTimestamp( TEST_TIMESTAMP_MESSAGE, out timestamp );
			Assert.AreEqual( TEST_TIMESTAMP, timestamp );
		}

		[Test]
		public void TimestampParser_GivenTimestampOutOfRangeMessage_ReturnsTrue() {
			var parser = new TimestampParser();
			long timestamp;
			bool result = parser.TryParseTimestamp( TEST_TIMESTAMP_MESSAGE, out timestamp );
			Assert.IsTrue( result );
		}

		[Test]
		public void TimestampParser_GivenUnparsableMessage_ReturnsFalse() {
			var parser = new TimestampParser();
			long timestamp;
			bool result = parser.TryParseTimestamp( "timestamp", out timestamp );
			Assert.IsFalse( result );
		}

		private const long IF_MORE_BASE_YEAR_MAY_BE_WRONG = 8L * 1000L * 1000L * 1000L * 1000L;
		private const long IF_LESS_UNITS_ARE_WRONG = 1300L * 1000L * 1000 * 1000L;
		private const string TEST_KEY = "TestAppKey";
		private const string TEST_DATA = "http://univ.edu/";
		private const string TEST_DATA_2 = "GET&/d2l/api/lp/1.0/organization/info&1318600982";
		private const string TEST_DATA_3 = "GET&/d2l/api/lp/1.0/organization/info&1318603541";

		private const string TEST_HASH = "Yxu5Irp07ZXavNvU5UbuPrxrggWnvaOczXtCPeAKmY8";
		private const string TEST_HASH_2 = "Ow-WGtpYff6kZBVTb4P-DYOMk43FdVVp8HtiS3nGnfY";
		private const string TEST_HASH_3 = "kgefiSDw01hI1yyOvtPs_RLEgk3uYcVyKS2XJXUfCTM";

		private const string TEST_TIMESTAMP_MESSAGE = "Timestamp out of range\r\n1319554666\r\n";
		private const long TEST_TIMESTAMP = 1319554666L;
	}
}
