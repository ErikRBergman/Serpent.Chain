// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.AppendMany;

    /// <summary>
    ///     The append many decorator extensions
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
                innerMessageHandler => (message, token) => Task.WhenAll(
                    innerMessageHandler(message, token),
                    AppendManyIfAsync(
                        new AppendManyAsyncParameters<TMessageType>
                            {
                                InnerMessageHandler = innerMessageHandler,
                                Predicate = predicate,
                                MessageSelector = messageSelector,
                                Message = message,
                                CancellationToken = token,
                                IsRecursive = isRecursive
                            })));
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
                innerMessageHandler => (message, token) => Task.WhenAll(
                    innerMessageHandler(message, token),
                    AppendManyIfAsync(
                        new AppendManyParameters<TMessageType>
                            {
                                Predicate = predicate,
                                MessageSelector = messageSelector,
                                InnerMessageHandler = innerMessageHandler,
                                Message = message,
                                IsRecursive = isRecursive
                            })));
        }

        private static Task AppendManyIfAsync<TMessageType>(AppendManyParameters<TMessageType> manyParameters)
        {
            if (manyParameters.Predicate(manyParameters.Message) == false)
            {
                return Task.CompletedTask;
            }

            var newMessages = manyParameters.MessageSelector(manyParameters.Message);
            if (newMessages == null)
            {
                return Task.CompletedTask;
            }

            var newMessageTask = Task.WhenAll(newMessages.Select(message => manyParameters.InnerMessageHandler(message, manyParameters.Token)));

            if (manyParameters.IsRecursive)
            {
                return Task.WhenAll(newMessageTask, Task.WhenAll(newMessages.Select(newMessage => AppendManyIfAsync(manyParameters.CloneForMessage(newMessage)))));
            }

            return newMessageTask;
        }

        private static async Task AppendManyIfAsync<TMessageType>(AppendManyAsyncParameters<TMessageType> parameters)
        {
            if (!await parameters.Predicate(parameters.Message, parameters.CancellationToken).ConfigureAwait(false))
            {
                return;
            }

            var newMessages = await parameters.MessageSelector(parameters.Message, parameters.CancellationToken).ConfigureAwait(false);
            if (newMessages == null)
            {
                return;
            }

            var newMessagesTask = Task.WhenAll(newMessages.Select(message => parameters.InnerMessageHandler(message, parameters.CancellationToken)));

            if (!parameters.IsRecursive)
            {
                await newMessagesTask.ConfigureAwait(false);
                return;
            }

            await Task.WhenAll(newMessagesTask, Task.WhenAll(newMessages.Select(newMessage => AppendManyIfAsync(parameters.CloneForMessage(newMessage)))));
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