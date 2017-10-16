namespace Serpent.Common.MessageBus.Extras
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus;

    /// <summary>
    /// A container for permanent subscriptions
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class PermanentSubscriptions<TMessageType> : IMessageBusSubscriptions<TMessageType>
    {
        private readonly IMessageBusSubscriptions<TMessageType> busSubscriptions;
        private readonly ConcurrentBag<IMessageBusSubscription> subscriptions = new ConcurrentBag<IMessageBusSubscription>();

        /// <summary>
        /// Creates an instance of the permanent subscriptions
        /// </summary>
        /// <param name="busSubscriptions">The bus subscriptions</param>
        public PermanentSubscriptions(IMessageBusSubscriptions<TMessageType> busSubscriptions)
        {
            this.busSubscriptions = busSubscriptions;
        }

        /// <summary>
        /// Subscribe to a message bus
        /// </summary>
        /// <param name="handlerFunc">
        /// The 
        /// </param>
        /// <returns>
        /// The <see cref="IMessageBusSubscription"/>.
        /// </returns>
        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            var subscription = this.busSubscriptions.Subscribe(handlerFunc);
            this.subscriptions.Add(subscription);
            return subscription;
        }
    }
}