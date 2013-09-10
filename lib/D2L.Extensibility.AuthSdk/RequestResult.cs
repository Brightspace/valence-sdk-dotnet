namespace D2L.Extensibility.AuthSdk {

    /// <summary>
    /// Indicates the result of a rest request made on a D2L API server.
    /// </summary>
	public enum RequestResult {
		BAD_REQUEST,
		NOT_FOUND,
		INTERNAL_SERVER_ERROR,
		RESULT_INVALID_SIG,
		RESULT_INVALID_TIMESTAMP,
		RESULT_NO_PERMISSION,
		RESULT_UNKNOWN
	}
}
