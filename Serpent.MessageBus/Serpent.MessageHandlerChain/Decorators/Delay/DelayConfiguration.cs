namespace Serpent.MessageHandlerChain.Decorators.Delay
{
    using System;

    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    /// The delay decorator configuration
    /// </summary>
    [WireUpConfigurationName("Delay")]
    public sealed class DelayConfiguration
    {
        /// <summary>
        /// The delay to await before a message is passed on
        /// </summary>
        public TimeSpan Delay { get; set; }
    }
}