namespace Serpent.Common.MessageBus.Helpers
{
    using System;
    using System.Threading.Tasks;

    internal class NullMessageSubscriber<TMessageType> : IMessageBusSubscriber<TMessageType>
    {
        public static NullMessageSubscriber<TMessageType> Default { get; } = new NullMessageSubscriber<TMessageType>();

        public IMessageBusSubscription Subscribe(Func<TMessageType, Task> invocationFunc)
        {
            return new NullMessageBusSubscription();
        }
    }
}