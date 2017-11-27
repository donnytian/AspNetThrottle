using System;
using System.Threading;

namespace AspNetThrottle
{
    /// <summary>
    /// Counter for requests from a certain client.
    /// </summary>
    public class RequestCounter
    {
        private long _totalRequest;

        /// <summary>
        /// Gets or sets the time stamp that the counter start.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the total request count.
        /// </summary>
        public long TotalRequests
        {
            get => _totalRequest;
            set => _totalRequest = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the counter already exceed the limit.
        /// </summary>
        public bool LimitExceeded { get; set; }

        /// <summary>
        /// Increases by one for TotalRequests.
        /// </summary>
        public void Increment()
        {
            Interlocked.Increment(ref _totalRequest);
        }
    }
}
