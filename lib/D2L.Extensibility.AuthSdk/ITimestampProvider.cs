namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// Provides a way to get the current time in milliseconds
    /// </summary>
	public interface ITimestampProvider {

        /// <summary>
        /// Returns the current time in milliseconds
        /// </summary>
        /// <returns>The current time in milliseconds</returns>
		long GetCurrentTimestampInMilliseconds();
	}
}
