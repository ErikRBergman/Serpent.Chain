namespace Serpent.Chain.Decorators.Delay
{
    using System;

    using Serpent.Chain.WireUp;

    /// <summary>
    /// The delay decorator configuration
    /// </summary>
#pragma warning disable CC0021 // Use nameof
    [WireUpConfigurationName("Delay")]
    public sealed class DelayConfiguration
    {
        /// <summary>
        /// The delay to await before a message is passed on
        /// </summary>
        public TimeSpan Delay { get; set; }
    }
}