namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class ConcurrentAttribute : WireUpAttribute
    {
        public ConcurrentAttribute(int maxNumberOfConcurrentMessages)
        {
            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
        }

        public int MaxNumberOfConcurrentMessages { get; }
    }
}