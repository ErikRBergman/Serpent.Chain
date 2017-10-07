// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class ConcurrentAttribute : WireUpAttribute
    {
        public ConcurrentAttribute(int maxNumberOfConcurrentMessages)
        {
            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
        }

        public int MaxNumberOfConcurrentMessages { get; }
    }
}