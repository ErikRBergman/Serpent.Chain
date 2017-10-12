namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    [WireUpConfigurationName("LimitedThroughput")]
    public class LimitedThroughputConfiguration
    {
        public int MaxNumberOfMessagesPerPeriod { get; set; }

        public TimeSpan Period { get; set; }
    }
}