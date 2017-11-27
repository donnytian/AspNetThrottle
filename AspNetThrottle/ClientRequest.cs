namespace AspNetThrottle
{
    /// <summary>
    /// Represents a client request.
    /// </summary>
    public class ClientRequest
    {
        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the request path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the request HTTP verb.
        /// </summary>
        public string HttpVerb { get; set; }
    }
}
