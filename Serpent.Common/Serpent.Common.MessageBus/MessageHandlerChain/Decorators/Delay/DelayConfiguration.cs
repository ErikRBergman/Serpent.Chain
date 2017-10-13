namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Delay
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    [WireUpConfigurationName("Delay")]
    public class DelayConfiguration
    {
        public TimeSpan Delay { get; set; }
    }
}