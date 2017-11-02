// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;

    using Serpent.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    ///     The retry decorator wire up attribute
    /// </summary>
    public class RetryAttribute : WireUpAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryAttribute" /> class.
        /// </summary>
        /// <param name="maxNumberOfAttempts">
        ///     The maximum number of attempts (not retries)
        /// </param>
        /// <param name="retryDelayInSeconds">
        ///     The delay between attempts in seconds, if an attempt fails. Fractions can be used. For example 1, or 0.4
        /// </param>
        public RetryAttribute(int maxNumberOfAttempts, double retryDelayInSeconds)
        {
            this.MaxNumberOfAttempts = maxNumberOfAttempts;
            this.RetryDelay = TimeSpan.FromSeconds(retryDelayInSeconds);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryAttribute" /> class.
        /// </summary>
        /// <param name="maxNumberOfAttempts">
        ///     The maximum number of attempts (not retries)
        /// </param>
        /// <param name="retryDelayInText">
        ///     The delay between attempts, if an attempt fails
        /// </param>
        public RetryAttribute(int maxNumberOfAttempts, string retryDelayInText)
        {
            this.MaxNumberOfAttempts = maxNumberOfAttempts;
            this.RetryDelay = TimeSpan.Parse(retryDelayInText);
        }

        /// <summary>
        /// The maximum number of attempts (first attempt + retries)
        /// </summary>
        public int MaxNumberOfAttempts { get; }

        /// <summary>
        /// The delay between retries
        /// </summary>
        public TimeSpan RetryDelay { get; }

        /// <summary>
        /// Set this property to true to use IMessageHandlerRetry on the message handler to handle retries
        /// </summary>
        public bool UseIMessageHandlerRetry { get; set; } = true;
    }
}