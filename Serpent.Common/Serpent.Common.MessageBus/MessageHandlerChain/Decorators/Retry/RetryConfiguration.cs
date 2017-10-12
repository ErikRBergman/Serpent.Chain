namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    [WireUpConfigurationName("Retry")]
    public class RetryConfiguration
    {
        public bool UseIMessageHandlerRetry { get; set; } = true;

        public int MaxNumberOfRetries { get; set; }

        public TimeSpan RetryDelay { get; set; }
    }
}