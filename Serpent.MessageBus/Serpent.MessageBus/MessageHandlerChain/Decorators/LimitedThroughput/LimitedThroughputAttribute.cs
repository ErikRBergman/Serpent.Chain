// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;

    using Serpent.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The limited throughput decorator wire up attribute
    /// </summary>
    public class LimitedThroughputAttribute : WireUpAttribute
    {
        /// <summary>
        /// Initializes a new instance of the limited throughput decorator attribute
        /// </summary>
        /// <param name="maxNumberOfMessagesPerPeriod">The maximum number of messages per period</param>
        /// <param name="periodText">The period timespan (in text), "D.HH:mm:ss.mmm"</param>
        public LimitedThroughputAttribute(int maxNumberOfMessagesPerPeriod, string periodText)
        {
            this.MaxNumberOfMessagesPerPeriod = maxNumberOfMessagesPerPeriod;
            this.Period = TimeSpan.Parse(periodText);
        }

        /// <summary>
        /// Initializes a new instance of the limited throughput decorator attribute
        /// </summary>
        /// <param name="maxNumberOfMessagesPerPeriod">The maximum number of messages per period</param>
        public LimitedThroughputAttribute(int maxNumberOfMessagesPerPeriod)
        {
            this.MaxNumberOfMessagesPerPeriod = maxNumberOfMessagesPerPeriod;
            this.Period = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// The maximum number of messages per period
        /// </summary>
        public int MaxNumberOfMessagesPerPeriod { get; }
    
        /// <summary>
        /// The period span
        /// </summary>
        public TimeSpan Period { get; }
    }
}