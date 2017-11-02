// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using System;

    using Serpent.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The retry decorator wire up configuration
    /// </summary>
    [WireUpConfigurationName("Retry")]
    public class RetryConfiguration
    {
        /// <summary>
        /// Set to true to use the IMessageHandlerRetry of the handler
        /// </summary>
        public bool UseIMessageHandlerRetry { get; set; } = true;

        /// <summary>
        /// The maximum number of attempts
        /// </summary>
        public int MaxNumberOfRetries { get; set; }

        /// <summary>
        /// The delay between failed attempts
        /// </summary>
        public TimeSpan RetryDelay { get; set; }
    }
}