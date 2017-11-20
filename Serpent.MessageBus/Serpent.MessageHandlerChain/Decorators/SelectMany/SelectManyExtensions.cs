// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.SelectMany;

    /// <summary>
    /// The .SelectMany() decorator extensions type
    /// </summary>
    public static class SelectManyExtensions
    {
        /// <summary>
        /// Projects each element of a sequence, producing a new chain with the inner element type
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        /// The builder
        /// </param>
        /// <param name="selector">
        /// A selector
        /// </param>
        /// <returns>
        /// The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;"/>.
        /// </returns>
        public static IMessageHandlerChainBuilder<TNewMessageType> SelectMany<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, IEnumerable<TNewMessageType>> selector)
        {
            var newMessagesTypeBuilder = new MessageHandlerChainBuilder<TNewMessageType>();

            messageHandlerChainBuilder.Handle(
                services =>
                    {
                        var handler = new SelectManyDecorator<TMessageType, TNewMessageType>(newMessagesTypeBuilder, selector);
                        services.BuildNotification.AddNotification(handler.MessageHandlerChainBuilt);
                        return handler.HandleMessageAsync;
                    });

            return newMessagesTypeBuilder;
        }

        /// <summary>
        /// Projects each element of a sequence, producing a new chain with the inner element type
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        /// The builder
        /// </param>
        /// <param name="selector">
        /// A selector
        /// </param>
        /// <returns>
        /// The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;"/>.
        /// </returns>
        public static IMessageHandlerChainBuilder<TNewMessageType> SelectMany<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<IEnumerable<TNewMessageType>>> selector)
        {
            var newMessagesTypeBuilder = new MessageHandlerChainBuilder<TNewMessageType>();

            messageHandlerChainBuilder.Handle(
                services =>
                    {
                        var handler = new SelectManyAsyncDecorator<TMessageType, TNewMessageType>(newMessagesTypeBuilder, selector);
                        services.BuildNotification.AddNotification(handler.MessageHandlerChainBuilt);
                        return handler.HandleMessageAsync;
                    });

            return newMessagesTypeBuilder;
        }
    }
}