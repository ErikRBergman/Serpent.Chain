namespace Serpent.MessageHandlerChain.Decorators.Concurrent
{
    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    /// Configuration to wire up ".Concurrent()" 
    /// </summary>
    [WireUpConfigurationName("Concurrent")]
    public sealed class ConcurrentConfiguration
    {
        /// <summary>
        /// The maximum number of concurrent messages being handled
        /// </summary>
        public int MaxNumberOfConcurrentMessages { get; set;  }
    }
}