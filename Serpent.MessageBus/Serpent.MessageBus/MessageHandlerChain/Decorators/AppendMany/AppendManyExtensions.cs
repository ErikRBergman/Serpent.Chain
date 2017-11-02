// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The append many decorator extensions
    /// </summary>
    public static class AppendManyExtensions
    {
        private static readonly Task<bool> TrueTask = Task.FromResult(true);

        /// <summary>
        ///     Append a range of messages for each message passed through
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

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                var chainedMessageTask = innerMessageHandler(message, token);
                                return Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, (msg, _) => messageSelector(msg), message, token));
                            };
                    });
        }

        /// <summary>
        ///     Append a range of messages for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                var chainedMessageTask = innerMessageHandler(message, token);
                                return Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, messageSelector, message, token));
                            };
                    });
        }

        /// <summary>
        ///     Append a range of messages for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="predicate">Append only if this predicate returns true</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <param name="isRecursive">Set to true to run the new message through this Append</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task<bool>> predicate,
            Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector,
            bool isRecursive = false)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler => 
                (message, token) => Task.WhenAll(innerMessageHandler(message, token), AppendManyIfAsync(innerMessageHandler, predicate, messageSelector, message, token, isRecursive)));
        }

        /// <summary>
        ///     Append a range of messages for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <param name="isRecursive">Set to true to run the new message through this Append</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector,
            bool isRecursive)
        {
            return messageHandlerChainBuilder.AppendMany((message, token) => TrueTask, messageSelector, isRecursive);
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

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                var originalMessageTask = innerMessageHandler(message, token);
                                var newMessages = messageSelector(message);
                                if (newMessages == null)
                                {
                                    return originalMessageTask;
                                }

                                var newMessagesTask = Task.WhenAll(newMessages.Select(msg => innerMessageHandler(msg, token)));
                                return Task.WhenAll(originalMessageTask, newMessagesTask);
                            };
                    });
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to return the new messages</param>
        /// <param name="isRecursive">True if the predicate and conditional append should apply to the sub messages</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, IEnumerable<TMessageType>> messageSelector,
            bool isRecursive)
        {
            return messageHandlerChainBuilder.AppendMany(message => true, messageSelector, isRecursive);
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

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler => (message, token) => Task.WhenAll(innerMessageHandler(message, token), AppendManyIfAsync(predicate, messageSelector, innerMessageHandler, message, token, isRecursive)));
        }

        private static Task AppendManyIfAsync<TMessageType>(
            Func<TMessageType, bool> predicate,
            Func<TMessageType, IEnumerable<TMessageType>> messageSelector,
            Func<TMessageType, CancellationToken, Task> innerMessageHandler,
            TMessageType messages,
            CancellationToken token,
            bool isRecursive)
        {
            if (predicate(messages) == false)
            {
                return Task.CompletedTask;
            }

            var newMessages = messageSelector(messages);
            if (newMessages == null)
            {
                return Task.CompletedTask;
            }

            var newMessageTask = Task.WhenAll(newMessages.Select(message => innerMessageHandler(message, token)));

            if (isRecursive)
            {
                return Task.WhenAll(
                    newMessageTask,
                    Task.WhenAll(newMessages.Select(newMessage => AppendManyIfAsync(predicate, messageSelector, innerMessageHandler, newMessage, token, true))));
            }

            return newMessageTask;
        }

        private static async Task AppendManyIfAsync<TMessageType>(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            Func<TMessageType, CancellationToken, Task<bool>> predicate,
            Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector,
            TMessageType originalMessage,
            CancellationToken token,
            bool isRecursive)
        {
            if (!await predicate(originalMessage, token).ConfigureAwait(false))
            {
                return;
            }

            var newMessages = await messageSelector(originalMessage, token).ConfigureAwait(false);
            if (newMessages == null)
            {
                return;
            }

            var newMessagesTask = Task.WhenAll(newMessages.Select(message => messageHandler(message, token)));

            if (!isRecursive)
            {
                await newMessagesTask.ConfigureAwait(false);
                return;
            }

            await Task.WhenAll(newMessagesTask, Task.WhenAll(newMessages.Select(newMessage => AppendManyIfAsync(messageHandler, predicate, messageSelector, newMessage, token, true))));
        }

        private static async Task InnerMessageHandlerAsync<TMessageType>(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector,
            TMessageType originalMessage,
            CancellationToken token)
        {
            var newMessages = await messageSelector(originalMessage, token).ConfigureAwait(false);
            if (newMessages != null)
            {
                await Task.WhenAll(newMessages.Select(message => messageHandler(message, token))).ConfigureAwait(false);
            }
        }
    }
}