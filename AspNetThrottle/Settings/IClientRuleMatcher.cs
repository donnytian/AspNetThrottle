namespace AspNetThrottle
{
    /// <summary>
    /// Provides services to help finding matching rules for client ID/ IP.
    /// </summary>
    public interface IClientRuleMatcher
    {
        /// <summary>
        /// Checks if the client ID is white-listed.
        /// </summary>
        /// <param name="clientId">The client ID string.</param>
        /// <returns>True if the ID is white-listed; otherwise false.</returns>
        bool IsWhitelisted(string clientId);

        /// <summary>
        /// Get client rules by the current matcher.
        /// </summary>
        /// <param name="clientId">The client ID string.</param>
        /// <returns>The array of matching rule; empty array if nothing matched.</returns>
        ThrottleRule[] GetClientRules(string clientId);
    }
}
