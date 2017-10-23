namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Delay
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The delay decorator configuration
    /// </summary>
    [WireUpConfigurationName("Delay")]
    public class DelayConfiguration
    {
        /// <summary>
        /// The delay to await before a message is passed on
        /// </summary>
        public TimeSpan Delay { get; set; }
    }
}