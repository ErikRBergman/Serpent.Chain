namespace Serpent.Common.MessageBus.Helpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class NullMessageSubscriptions<TMessageType> : IMessageBusSubscriptions<TMessageType>
    {
        private NullMessageSubscriptions()
        {
            
        }

        public static NullMessageSubscriptions<TMessageType> Default { get; } = new NullMessageSubscriptions<TMessageType>();

        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            return NullMessageBusSubscription.Default;
        }
    }
}