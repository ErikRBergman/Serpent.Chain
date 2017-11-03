// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.Append;

    /// <summary>
    ///     The append decorator extensions
    /// </summary>
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

            return messageHandlerChainBuilder.AddDecorator(
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

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler => (message, token) => Task.WhenAll(
                    innerMessageHandler(message, token),
                    AppendIfAsync(
                        new AppendAsyncParameters<TMessageType>
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

            return messageHandlerChainBuilder.AddDecorator(
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

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler => (message, token) => Task.WhenAll(
                    innerMessageHandler(message, token),
                    AppendIfAsync(
                        new AppendParameters<TMessageType>
                            {
                                Predicate = predicate,
                                MessageSelector = messageSelector,
                                InnerMessageHandler = innerMessageHandler,
                                Message = message,
                                CancellationToken = token,
                                IsRecursive = isRecursive
                            })));
        }

        private static Task AppendIfAsync<TMessageType>(AppendParameters<TMessageType> parameters)
        {
            if (parameters.Predicate(parameters.Message) == false)
            {
                return Task.CompletedTask;
            }

            var newMessage = parameters.MessageSelector(parameters.Message);
            var newMessageTask = parameters.InnerMessageHandler(newMessage, parameters.CancellationToken);

            if (parameters.IsRecursive)
            {
                return Task.WhenAll(newMessageTask, AppendIfAsync(parameters.CloneForMessage(newMessage)));
            }

            return newMessageTask;
        }

        private static async Task AppendIfAsync<TMessageType>(AppendAsyncParameters<TMessageType> parameters)
        {
            if (await parameters.Predicate(parameters.Message).ConfigureAwait(false))
            {
                var newMessage = await parameters.MessageSelector(parameters.Message).ConfigureAwait(false);
                if (parameters.IsRecursive)
                {
                    await Task.WhenAll(parameters.InnerMessageHandler(newMessage, parameters.CancellationToken), AppendIfAsync(parameters.CloneForMessage(newMessage)));
                }
                else
                {
                    await parameters.InnerMessageHandler(newMessage, parameters.CancellationToken).ConfigureAwait(false);
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