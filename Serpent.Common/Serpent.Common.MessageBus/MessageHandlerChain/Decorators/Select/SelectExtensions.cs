// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Select;

    public static class SelectExtensions
    {
        /// <summary>
        /// Select a new type of the rest of the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The current message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        /// The mhc builder
        /// </param>
        /// <param name="selector">
        /// The selector method
        /// </param>
        /// <returns>
        /// The <see cref="IMessageHandlerChainBuilder&lt;TNewMessageType&gt;"/>.
        /// </returns>
        public static IMessageHandlerChainBuilder<TNewMessageType> Select<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TNewMessageType> selector)
        {
            return new SelectDecorator<TMessageType, TNewMessageType>(messageHandlerChainBuilder, selector).NewMessageHandlerChainBuilder;
        }

        /// <summary>
        /// Select a new type of the rest of the message handler chain with an async selector
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The current message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        /// The mhc builder
        /// </param>
        /// <param name="selector">
        /// The async selector method
        /// </param>
        /// <returns>
        /// The <see cref="IMessageHandlerChainBuilder&lt;TNewMessageType&gt;"/>.
        /// </returns>
        public static IMessageHandlerChainBuilder<TNewMessageType> Select<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<TNewMessageType>> selector)
        {
            return new SelectAsyncDecorator<TMessageType, TNewMessageType>(messageHandlerChainBuilder, selector).NewMessageHandlerChainBuilder;
        }
    }
}