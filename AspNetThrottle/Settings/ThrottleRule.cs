using System;

namespace AspNetThrottle
{
    /// <summary>
    /// Configuration rule for throttle.
    /// </summary>
    public class ThrottleRule
    {
        private string _period;

        /// <summary>
        /// Gets or sets HTTP verb and path.
        /// </summary>
        /// <example>
        /// get:/api/values
        /// *:/api/values
        /// *
        /// </example>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets throttle period as in 1s, 1m, 1h.
        /// </summary>
        public string Period
        {
            get => _period;

            set
            {
                PeriodTimespan = ParseToTimeSpan(value);
                _period = value;
            }
        }

        /// <summary>
        /// Gets throttle period as <see cref="TimeSpan"/>.
        /// </summary>
        public TimeSpan? PeriodTimespan { get; private set; }

        /// <summary>
        /// Gets or sets the maximum number of requests that a client can make.
        /// </summary>
        public long Limit { get; set; }

        /// <summary>
        /// Parses to <see cref="TimeSpan"/> from a string.
        /// </summary>
        /// <param name="period">The period string representing a time span.</param>
        /// <returns>The parsed <see cref="TimeSpan"/>.</returns>
        public static TimeSpan ParseToTimeSpan(string period)
        {
            var l = period.Length - 1;
            var value = period.Substring(0, l);
            var type = period.Substring(l, 1);

            switch (type)
            {
                case "d": return TimeSpan.FromDays(double.Parse(value));
                case "h": return TimeSpan.FromHours(double.Parse(value));
                case "m": return TimeSpan.FromMinutes(double.Parse(value));
                case "s": return TimeSpan.FromSeconds(double.Parse(value));
                default: throw new FormatException($"{period} can't be converted to TimeSpan, unknown type {type}");
            }
        }
    }
}
