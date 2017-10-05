// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class AppendExtensions
    {
        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<TMessageType>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return async (message, token) =>
                            {
                                var chainedMessageTask = innerMessageHandler(message, token);
                                await Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, messageSelector, message, token));
                            };
                    });
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="predicate">Append only if this predicate returns true</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <param name="isRecursive">
        ///     Set to true to run the selected message through this Append recursively as long as the
        ///     predicate returns true
        /// </param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> predicate,
            Func<TMessageType, Task<TMessageType>> messageSelector,
            bool isRecursive = false)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler => (message, token) => Task.WhenAll(innerMessageHandler(message, token), AppendIfAsync(innerMessageHandler, predicate, messageSelector, message, token, isRecursive)));
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TMessageType> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                var originalMessageTask = innerMessageHandler(message, token);
                                var newMessageTask = innerMessageHandler(messageSelector(message), token);
                                return Task.WhenAll(originalMessageTask, newMessageTask);
                            };
                    });
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The chain message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The mch builder
        /// </param>
        /// <param name="predicate">
        ///     Append only if this predicate returns true
        /// </param>
        /// <param name="messageSelector">
        ///     The function used to create the new message
        /// </param>
        /// <param name="isRecursive">
        ///     Set to true to run the selected message through this Append recursively as long as the
        ///     predicate returns true
        /// </param>
        /// <returns>
        ///     The same mch builder
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate,
            Func<TMessageType, TMessageType> messageSelector,
            bool isRecursive = false)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate may not be null");
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler => (message, token) => Task.WhenAll(innerMessageHandler(message, token), AppendIfAsync(predicate, messageSelector, innerMessageHandler, message, token, isRecursive)));
        }

        private static Task AppendIfAsync<TMessageType>(
            Func<TMessageType, bool> predicate,
            Func<TMessageType, TMessageType> messageSelector,
            Func<TMessageType, CancellationToken, Task> innerMessageHandler,
            TMessageType message,
            CancellationToken token,
            bool isRecursive)
        {
            if (predicate(message) == false)
            {
                return Task.CompletedTask;
            }

            var newMessage = messageSelector(message);
            var newMessageTask = innerMessageHandler(newMessage, token);

            if (isRecursive)
            {
                return Task.WhenAll(newMessageTask, AppendIfAsync(predicate, messageSelector, innerMessageHandler, newMessage, token, true));
            }

            return newMessageTask;
        }

        private static async Task AppendIfAsync<TMessageType>(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            Func<TMessageType, Task<bool>> predicate,
            Func<TMessageType, Task<TMessageType>> messageSelector,
            TMessageType originalMessage,
            CancellationToken token,
            bool isRecursive)
        {
            if (await predicate(originalMessage).ConfigureAwait(false))
            {
                var newMessage = await messageSelector(originalMessage).ConfigureAwait(false);
                if (isRecursive)
                {
                    await Task.WhenAll(messageHandler(newMessage, token), AppendIfAsync(messageHandler, predicate, messageSelector, newMessage, token, true));
                }
                else
                {
                    await messageHandler(newMessage, token).ConfigureAwait(false);
                }
            }
        }

        private static async Task InnerMessageHandlerAsync<TMessageType>(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            Func<TMessageType, Task<TMessageType>> messageSelector,
            TMessageType originalMessage,
            CancellationToken token)
        {
            var newMessage = await messageSelector(originalMessage).ConfigureAwait(false);
            await messageHandler(newMessage, token).ConfigureAwait(false);
        }
    }
}