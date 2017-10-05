// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class PrependExtensions
    {
        /// <summary>
        /// Prepend a message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messagePrependFunc">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Prepend<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<TMessageType>> messagePrependFunc)
        {
            if (messagePrependFunc == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                var prependMessageTask = InnerMessageHandlerAsync(innerMessageHandler, messagePrependFunc, message, token);
                                var chainedMessageTask = innerMessageHandler(message, token);
                                return Task.WhenAll(chainedMessageTask, prependMessageTask);
                            };
                    });
        }

        /// <summary>
        /// Prepend a message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageAppendFunc">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Prepend<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TMessageType> messageAppendFunc)
        {
            if (messageAppendFunc == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                var newMessageTask = innerMessageHandler(messageAppendFunc(message), token);
                                var originalMessageTask = innerMessageHandler(message, token);
                                return Task.WhenAll(originalMessageTask, newMessageTask);
                            };
                    });
        }

        private static async Task InnerMessageHandlerAsync<TMessageType>(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            Func<TMessageType, Task<TMessageType>> prependMessageFunc,
            TMessageType originalMessage,
            CancellationToken token)
        {
            var newMessage = await prependMessageFunc(originalMessage).ConfigureAwait(false);
            await messageHandler(newMessage, token).ConfigureAwait(false);
        }
    }
}