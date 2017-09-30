// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class AppendManyExtensions
    {
        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return async message =>
                            {
                                var chainedMessageTask = innerMessageHandler(message);
                                await Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, messageSelector, message));
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
        /// <param name="isRecursive">Set to true to run the new message through this Append</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> predicate,
            Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector,
            bool isRecursive = false)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler => message => Task.WhenAll(
                innerMessageHandler(message),
                AppendIfAsync(innerMessageHandler, predicate, messageSelector, message, isRecursive)));
        }


        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, IEnumerable<TMessageType>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return async message =>
                            {
                                var originalMessageTask = innerMessageHandler(message);
                                var newMessages = messageSelector(message);
                                var newMessagesTask = Task.WhenAll(newMessages.Select(innerMessageHandler));
                                await Task.WhenAll(originalMessageTask, newMessagesTask).ConfigureAwait(false);
                            };
                    });
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="predicate">Append only if this predicate returns true</param>
        /// <param name="messageSelector">The function used to return the new messages</param>
        /// <param name="isRecursive">True if the predicate and conditional append should apply to the sub messages</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate,
            Func<TMessageType, IEnumerable<TMessageType>> messageSelector,
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
                innerMessageHandler =>
                    message =>
                        Task.WhenAll(innerMessageHandler(message), AppendIfAsync(predicate, messageSelector, innerMessageHandler, message, isRecursive)));
        }

        private static Task AppendIfAsync<TMessageType>(Func<TMessageType, bool> predicate, Func<TMessageType, IEnumerable<TMessageType>> messageSelector, Func<TMessageType, Task> innerMessageHandler, TMessageType messages, bool isRecursive)
        {
            if (predicate(messages) == false)
            {
                return Task.CompletedTask;
            }

            var newMessages = messageSelector(messages);
            var newMessageTask = Task.WhenAll(newMessages.Select(innerMessageHandler));

            if (isRecursive)
            {
                return Task.WhenAll(newMessageTask, Task.WhenAll(newMessages.Select(newMessage => AppendIfAsync(predicate, messageSelector, innerMessageHandler, newMessage, true))));
            }

            return newMessageTask;
        }

        private static async Task AppendIfAsync<TMessageType>(
            Func<TMessageType, Task> messageHandler,
            Func<TMessageType, Task<bool>> predicate,
            Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector,
            TMessageType originalMessage,
            bool isRecursive)
        {
            if (!await predicate(originalMessage).ConfigureAwait(false))
            {
                return;
            }

            var newMessages = await messageSelector(originalMessage).ConfigureAwait(false);

            var newMessagesTask = Task.WhenAll(newMessages.Select(messageHandler));

            if (!isRecursive)
            {
                await newMessagesTask.ConfigureAwait(false);
                return;
            }

            await Task.WhenAll(newMessagesTask, Task.WhenAll(newMessages.Select(newMessage => AppendIfAsync(messageHandler, predicate, messageSelector, newMessage, true))));


        }

        private static async Task InnerMessageHandlerAsync<TMessageType>(
            Func<TMessageType, Task> messageHandler,
            Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector,
            TMessageType originalMessage)
        {
            var newMessages = await messageSelector(originalMessage).ConfigureAwait(false);

            await Task.WhenAll(newMessages.Select(messageHandler)).ConfigureAwait(false);
        }
    }
}