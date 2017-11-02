namespace Serpent.MessageBus.BusPublishers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Helpers;

    internal class MessageHandlerPublishDispatch<TMessageType> : IMessageBusSubscriptions<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> InvocationFunc { get; private set; }

        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            this.InvocationFunc = handlerFunc;
            return NullMessageBusSubscription.Default;
        }
    }
}