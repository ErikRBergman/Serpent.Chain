namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    [WireUpConfigurationName("Concurrent")]
    public class ConcurrentConfiguration
    {
        public int MaxNumberOfConcurrentMessages { get; set;  }
    }
}