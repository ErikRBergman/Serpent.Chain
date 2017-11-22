// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.Append;

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
#pragma warning disable CC0031 // Check for null before calling a delegate
                                var chainedMessageTask = innerMessageHandler(message, token);
#pragma warning restore CC0031 // Check for null before calling a delegate
                                await Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, messageSelector, message, token)).ConfigureAwait(false);
                            };
                    });
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="configure">The action used to configure the builder</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<IAppendDecoratorBuilder<TMessageType>> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var builder = new AppendDecoratorBuilder<TMessageType>();
            configure(builder);
            return messageHandlerChainBuilder.AddDecorator(builder.BuildDecorator());
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
#pragma warning disable CC0031 // Check for null before calling a delegate
                                var originalMessageTask = innerMessageHandler(message, token);
                                var newMessageTask = innerMessageHandler(messageSelector(message), token);
#pragma warning restore CC0031 // Check for null before calling a delegate
                                return Task.WhenAll(originalMessageTask, newMessageTask);
                            };
                    });
        }
       
        private static async Task InnerMessageHandlerAsync<TMessageType>(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            Func<TMessageType, Task<TMessageType>> messageSelector,
            TMessageType originalMessage,
            CancellationToken token)
        {
#pragma warning disable CC0031 // Check for null before calling a delegate
            var newMessage = await messageSelector(originalMessage).ConfigureAwait(false);
            await messageHandler(newMessage, token).ConfigureAwait(false);
#pragma warning restore CC0031 // Check for null before calling a delegate
        }
    }
}