// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public static class AppendExtensions
    {
        /// <summary>
        /// Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageAppendFunc">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<TMessageType>> messageAppendFunc)
        {
            if (messageAppendFunc == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return async message =>
                            {
                                var chainedMessageTask = innerMessageHandler(message);
                                await Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, messageAppendFunc, message));
                            };
                    });
        }

        /// <summary>
        /// Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageAppendFunc">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
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
                        return async message =>
                            {
                                var originalMessageTask = innerMessageHandler(message);
                                var newMessageTask = innerMessageHandler(messageAppendFunc(message));
                                await Task.WhenAll(originalMessageTask, newMessageTask).ConfigureAwait(false);
                            };
                    });
        }

        private static async Task InnerMessageHandlerAsync<TMessageType>(
            Func<TMessageType, Task> messageHandler,
            Func<TMessageType, Task<TMessageType>> messageAppendFunc,
            TMessageType originalMessage)
        {
            var newMessage = await messageAppendFunc(originalMessage).ConfigureAwait(false);
            await messageHandler(newMessage).ConfigureAwait(false);
        }
    }
}