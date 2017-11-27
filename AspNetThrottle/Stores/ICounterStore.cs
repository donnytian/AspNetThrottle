using System;

namespace AspNetThrottle
{
    /// <summary>
    /// Represents a cache to store throttle counters.
    /// </summary>
    public interface ICounterStore
    {
        /// <summary>
        /// Checks if entry exists by specified ID.
        /// </summary>
        /// <param name="id">The ID string.</param>
        /// <returns>True for exists; otherwise false.</returns>
        bool Exists(string id);

        /// <summary>
        /// Gets entry by specified ID.
        /// </summary>
        /// <param name="id">The ID string.</param>
        /// <returns>The entry with specified ID; null if the entry does not existed.</returns>
        RequestCounter Get(string id);

        /// <summary>
        /// Gets or creates entry.
        /// </summary>
        /// <param name="id">The ID string.</param>
        /// <param name="createFunc">The function to create a new entry.</param>
        /// <param name="expirationTime">The expiration time for the new entry.</param>
        /// <returns>Object with specified ID.</returns>
        RequestCounter GetOrCreate(string id, Func<RequestCounter> createFunc, TimeSpan expirationTime);

        /// <summary>
        /// Removes entry by specified ID.
        /// </summary>
        /// <param name="id">The ID string.</param>
        void Remove(string id);

        /// <summary>
        /// Sets entry in store.
        /// </summary>
        /// <param name="id">The ID string.</param>
        /// <param name="value">The data to be set.</param>
        /// <param name="expirationTime">Expiration time.</param>
        void Set(string id, RequestCounter value, TimeSpan expirationTime);
    }
}
