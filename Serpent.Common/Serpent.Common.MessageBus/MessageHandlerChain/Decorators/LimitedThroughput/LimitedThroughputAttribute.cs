// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class LimitedThroughputAttribute : WireUpAttribute
    {
        public LimitedThroughputAttribute(int maxNumberOfMessagesPerPeriod, string periodText)
        {
            this.MaxNumberOfMessagesPerPeriod = maxNumberOfMessagesPerPeriod;
            this.Period = TimeSpan.Parse(periodText);
        }

        public LimitedThroughputAttribute(int maxNumberOfMessagesPerPeriod)
        {
            this.MaxNumberOfMessagesPerPeriod = maxNumberOfMessagesPerPeriod;
            this.Period = TimeSpan.FromSeconds(1);
        }

        public int MaxNumberOfMessagesPerPeriod { get; }

        public TimeSpan Period { get; }
    }
}