// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Serpent.Chain.WireUp;

    /// <summary>
    ///     The retry decorator wire up attribute
    /// </summary>
    public sealed class RetryAttribute : WireUpAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryAttribute" /> class.
        /// </summary>
        /// <param name="maxNumberOfAttempts">
        ///     The maximum number of attempts (not retries)
        /// </param>
        /// <param name="retryDelaysInSeconds">
        ///     The delays between attempts in seconds, if an attempt fails. Fractions can be used. For example 1, or 0.4. First delay is for the first attempt, and so on. The last is for all other attempts.
        /// </param>
        public RetryAttribute(int maxNumberOfAttempts, params double[] retryDelaysInSeconds)
        {
            this.MaxNumberOfAttempts = maxNumberOfAttempts;
            this.RetryDelays = retryDelaysInSeconds.Select(TimeSpan.FromSeconds);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryAttribute" /> class.
        /// </summary>
        /// <param name="maxNumberOfAttempts">
        ///     The maximum number of attempts (not retries)
        /// </param>
        /// <param name="retryDelaysInText">
        ///     The delay between attempts, if an attempt fails. First delay is for the first attempt, and so on. The last is for all other attempts.
        /// </param>
        public RetryAttribute(int maxNumberOfAttempts, params string[] retryDelaysInText)
        {
            this.MaxNumberOfAttempts = maxNumberOfAttempts;
            this.RetryDelays = retryDelaysInText.Select(TimeSpan.Parse);
        }

        /// <summary>
        /// The maximum number of attempts (first attempt + retries)
        /// </summary>
        public int MaxNumberOfAttempts { get; }

        /// <summary>
        /// The delay between retries
        /// </summary>
        public IEnumerable<TimeSpan> RetryDelays { get; }

        /// <summary>
        /// Set this property to true to use IMessageHandlerRetry on the message handler to handle retries
        /// </summary>
        public bool UseIMessageHandlerRetry { get; set; } = true;
    }
}