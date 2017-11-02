namespace Serpent.MessageBus.Extras
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
        private readonly IMessageBus<TMessageType> messageBus;

        /// <summary>
        /// Creates an instance of the subscriptions bridge
        /// </summary>
        /// <param name="messageBus">The message messageBus</param>
        public SubscriptionsBridge(IMessageBus<TMessageType> messageBus)
        {
            this.messageBus = messageBus;
        }

        /// <summary>
        /// Subscribe to a message bus
        /// </summary>
        /// <param name="handlerFunc">
        /// The handler function
        /// </param>
        /// <returns>
        /// The <see cref="IMessageBusSubscription"/>.
        /// </returns>
        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.messageBus.Subscribe(handlerFunc);
        }
    }
}