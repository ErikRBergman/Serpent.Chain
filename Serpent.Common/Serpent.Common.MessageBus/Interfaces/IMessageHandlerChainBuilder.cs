// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The message handler chain builder interface
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageHandlerChainBuilder<TMessageType>
    {
        /// <summary>
        /// Adds a decorator to the message handler chain builder
        /// </summary>
        /// <param name="addFunc">A function that returns the function to call when building the chain</param>
        /// <returns>The builder</returns>
        IMessageHandlerChainBuilder<TMessageType> Add(Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> addFunc);
        
        /// <summary>
        /// Set a handler and create a subscription
        /// </summary>
        /// <param name="handlerFunc">The handler to invoke</param>
        /// <returns>The subscription</returns>
        IMessageBusSubscription Handler(Func<TMessageType, CancellationToken, Task> handlerFunc);
    }
}