// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The .Prepend() decorator extensions
    /// </summary>
    public static class PrependExtensions
    {
        /// <summary>
        /// Prepend a message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="chainBuilder">The mch builder</param>
        /// <param name="messagePrependFunc">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IChainBuilder<TMessageType> Prepend<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task<TMessageType>> messagePrependFunc)
        {
            if (messagePrependFunc == null)
            {
                return chainBuilder;
            }

            return chainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                var prependMessageTask = InnerMessageHandlerAsync(innerMessageHandler, messagePrependFunc, message, token);
#pragma warning disable CC0031 // Check for null before calling a delegate
                                var chainedMessageTask = innerMessageHandler(message, token);
#pragma warning restore CC0031 // Check for null before calling a delegate
                                return Task.WhenAll(chainedMessageTask, prependMessageTask);
                            };
                    });
        }

        /// <summary>
        /// Prepend a message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="chainBuilder">The mch builder</param>
        /// <param name="messageAppendFunc">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IChainBuilder<TMessageType> Prepend<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, TMessageType> messageAppendFunc)
        {
            if (messageAppendFunc == null)
            {
                return chainBuilder;
            }

            return chainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                                var newMessageTask = innerMessageHandler(messageAppendFunc(message), token);
                                var originalMessageTask = innerMessageHandler(message, token);
#pragma warning restore CC0031 // Check for null before calling a delegate
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
#pragma warning disable CC0031 // Check for null before calling a delegate
            var newMessage = await prependMessageFunc(originalMessage).ConfigureAwait(false);
            await messageHandler(newMessage, token).ConfigureAwait(false);
#pragma warning restore CC0031 // Check for null before calling a delegate
        }
    }
}