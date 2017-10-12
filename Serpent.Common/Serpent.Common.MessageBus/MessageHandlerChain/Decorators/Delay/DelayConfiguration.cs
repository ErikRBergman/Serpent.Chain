namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Delay
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;
    using System;

    [WireUpConfigurationName("Delay")]
    public class DelayConfiguration
    {
        public TimeSpan Delay { get; set; }
    }
}