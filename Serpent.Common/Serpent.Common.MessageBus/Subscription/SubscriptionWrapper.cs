//// ReSharper disable CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The subscription wrapper type. Unsubscribes when disposed or runs out of scope
    /// </summary>
    public class SubscriptionWrapper : IMessageBusSubscription
    {
        private IMessageBusSubscription subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionWrapper"/> class. 
        /// </summary>
        /// <param name="subscription">
        /// The subscription
        /// </param>
        public SubscriptionWrapper(IMessageBusSubscription subscription)
        {
            this.subscription = subscription;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SubscriptionWrapper"/> class. 
        /// </summary>
        ~SubscriptionWrapper()
        {
            this.Dispose();
        }

        /// <summary>
        /// Creates a new subscription wrapper
        /// </summary>
        /// <param name="messageBusSubscription">The subscription</param>
        /// <returns>A subscription wrapper</returns>
        public static SubscriptionWrapper Create(IMessageBusSubscription messageBusSubscription)
        {
            return new SubscriptionWrapper(messageBusSubscription);
        }

        /// <summary>
        /// Creates a new subscription wrapper
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="subscriptions">
        /// The subscriptions
        /// </param>
        /// <param name="invocationFunc">
        /// The method to invoke
        /// </param>
        /// <param name="messageFilterFunc">
        /// The message Filter Func.
        /// </param>
        /// <returns>
        /// A subscription wrapper
        /// </returns>
        public static SubscriptionWrapper Create<TMessageType>(IMessageBusSubscriptions<TMessageType> subscriptions, Func<TMessageType, CancellationToken, Task> invocationFunc, Func<TMessageType, bool> messageFilterFunc = null)
        {
            IMessageBusSubscription subscription;

            if (messageFilterFunc != null)
            {
                subscription = subscriptions.Subscribe((message, token) => messageFilterFunc(message) ? invocationFunc(message, token) : Task.CompletedTask);
            }
            else
            {
                subscription = subscriptions.Subscribe(invocationFunc);
            }

            return new SubscriptionWrapper(subscription);
        }

        /// <summary>
        /// Unsubscribes to the message bus
        /// </summary>
        public void Unsubscribe()
        {
            this.subscription?.Dispose();
            this.subscription = null;
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            this.Unsubscribe();
        }
    }
}