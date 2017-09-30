// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
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
                        return async message =>
                            {
                                var prependMessageTask = InnerMessageHandlerAsync(innerMessageHandler, messagePrependFunc, message);
                                var chainedMessageTask = innerMessageHandler(message);
                                await Task.WhenAll(chainedMessageTask, prependMessageTask).ConfigureAwait(false);
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
                        return message =>
                            {
                                var newMessageTask = innerMessageHandler(messageAppendFunc(message));
                                var originalMessageTask = innerMessageHandler(message);
                                return Task.WhenAll(originalMessageTask, newMessageTask);
                            };
                    });
        }

        private static async Task InnerMessageHandlerAsync<TMessageType>(
            Func<TMessageType, Task> messageHandler,
            Func<TMessageType, Task<TMessageType>> prependMessageFunc,
            TMessageType originalMessage)
        {
            var newMessage = await prependMessageFunc(originalMessage).ConfigureAwait(false);
            await messageHandler(newMessage).ConfigureAwait(false);
        }
    }
}