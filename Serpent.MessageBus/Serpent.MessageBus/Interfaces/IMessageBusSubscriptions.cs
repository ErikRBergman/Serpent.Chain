// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The message bus subscriptions interface. Used to set up subscriptions.
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageBusSubscriptions<out TMessageType>
    {
        /// <summary>
        /// Creates a new subscription, invoking the handlerFunc method when a message is published
        /// </summary>
        /// <param name="handlerFunc">The method to invoke when a message is published</param>
        /// <returns>An interface that allows you unsubscribe</returns>
        IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc);
    }
}