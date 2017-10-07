// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class LimitedThroughputAttribute : WireUpAttribute
    {
        public LimitedThroughputAttribute(int maxNumberOfMessagesPerPeriod, string periodText)
        {
            this.MaxNumberOfMessagesPerPeriod = maxNumberOfMessagesPerPeriod;
            this.Period = TimeSpan.Parse(periodText);
        }

        public int MaxNumberOfMessagesPerPeriod { get; }

        public TimeSpan Period { get; }
    }
}