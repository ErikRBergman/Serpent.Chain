// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus.MessageHandlerChain
{
    using System;

    using Serpent.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The limited throughput decorator configuration
    /// </summary>
    [WireUpConfigurationName("LimitedThroughput")]
    public class LimitedThroughputConfiguration
    {
        /// <summary>
        /// The maximum number of messages to handler per period
        /// </summary>
        public int MaxNumberOfMessagesPerPeriod { get; set; }

        /// <summary>
        /// The period span
        /// </summary>
        public TimeSpan Period { get; set; }
    }
}