namespace Serpent.Common.MessageBus.Extras
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A bridge to allow registering IMessageBusSubscriptions to the same IMessageBus with simpler IOC containers, like the one in ASP NET Core
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct SubscriptionsBridge<TMessageType> : IMessageBusSubscriptions<TMessageType>
    {
        private readonly IMessageBus<TMessageType> bus;

        public SubscriptionsBridge(IMessageBus<TMessageType> bus)
        {
            this.bus = bus;
        }

        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.bus.Subscribe(handlerFunc);
        }
    }
}