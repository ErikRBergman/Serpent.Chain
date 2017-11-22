namespace Serpent.MessageHandlerChain.Decorators.Concurrent
{
    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    /// Configuration to wire up ".Concurrent()" 
    /// </summary>
#pragma warning disable CC0021 // Use nameof
    [WireUpConfigurationName("Concurrent")]
#pragma warning restore CC0021 // Use nameof
    public sealed class ConcurrentConfiguration
    {
        /// <summary>
        /// The maximum number of concurrent messages being handled
        /// </summary>
        public int MaxNumberOfConcurrentMessages { get; set;  }
    }
}