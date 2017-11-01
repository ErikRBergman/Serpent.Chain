//// ReSharper disable CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The subscription wrapper type. Unsubscribes when disposed or runs out of scope
    /// </summary>
    public struct MessageHandlerChainWrapper : IMessageHandlerChain
    {
        private IMessageHandlerChain messageHandlerChain;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerChainWrapper"/> class. 
        /// </summary>
        /// <param name="messageHandlerChain">
        /// The subscription
        /// </param>
        public MessageHandlerChainWrapper(IMessageHandlerChain messageHandlerChain)
        {
            this.messageHandlerChain = messageHandlerChain;
        }

        /// <summary>
        /// Creates a new subscription wrapper
        /// </summary>
        /// <param name="messageHandlerChain">The message handler chain</param>
        /// <returns>A subscription wrapper</returns>
        public static MessageHandlerChainWrapper Create(IMessageHandlerChain messageHandlerChain)
        {
            return new MessageHandlerChainWrapper(messageHandlerChain);
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
        ////public static MessageHandlerChainWrapper Create<TMessageType>(IMessageBusSubscriptions<TMessageType> subscriptions, Func<TMessageType, CancellationToken, Task> invocationFunc, Func<TMessageType, bool> messageFilterFunc = null)
        ////{
        ////    IMessageBusSubscription subscription;

        ////    if (messageFilterFunc != null)
        ////    {
        ////        subscription = subscriptions.Subscribe((message, token) => messageFilterFunc(message) ? invocationFunc(message, token) : Task.CompletedTask);
        ////    }
        ////    else
        ////    {
        ////        subscription = subscriptions.Subscribe(invocationFunc);
        ////    }

        ////    return new MessageHandlerChainWrapper(subscription);
        ////}

        /// <summary>
        /// Unsubscribes to the message bus
        /// </summary>
        public void Unsubscribe()
        {
            this.messageHandlerChain?.Dispose();
            this.messageHandlerChain = null;
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