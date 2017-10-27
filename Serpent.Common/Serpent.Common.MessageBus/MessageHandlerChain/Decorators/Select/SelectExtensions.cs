// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Select;

    /// <summary>
    /// The .Select() decorator extensions type
    /// </summary>
    public static class SelectExtensions
    {
        /// <summary>
        /// Projects each message to a new form
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
        /// A transform function to apply to each message
        /// </param>
        /// <returns>
        /// A builder of the new message type <see cref="IMessageHandlerChainBuilder&lt;TNewMessageType&gt;"/>.
        /// </returns>
        public static IMessageHandlerChainBuilder<TNewMessageType> Select<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TNewMessageType> selector)
        {
            return new SelectDecorator<TMessageType, TNewMessageType>(messageHandlerChainBuilder, selector).NewMessageHandlerChainBuilder;
        }

        /// <summary>
        /// Projects each message to a new form with async selector
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
        /// A transform function to apply to each message
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