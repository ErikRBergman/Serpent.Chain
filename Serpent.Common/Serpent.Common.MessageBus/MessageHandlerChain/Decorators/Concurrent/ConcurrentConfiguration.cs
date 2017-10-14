namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// Configuration to wire up ".Concurrent()" 
    /// </summary>
    [WireUpConfigurationName("Concurrent")]
    public class ConcurrentConfiguration
    {
        public int MaxNumberOfConcurrentMessages { get; set;  }
    }
}