// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public static class WhereExtensions
    {
        /// <summary>
        /// Filters messages based on a predicate
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="asyncPredicate">An async function to test each message for a condition</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Where<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> asyncPredicate)
        {
            if (asyncPredicate == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler =>
                {
                    return async (message, token) =>
                        {
                            if (await asyncPredicate(message).ConfigureAwait(false))
                            {
                                await innerMessageHandler(message, token).ConfigureAwait(false);
                            }
                        };
                });
        }

        /// <summary>
        /// Filters messages based on a predicate
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="predicate">An async function to test each message for a condition</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Where<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate)
        {
            if (predicate == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler => (message, cancellationToken) =>
                {
                    if (predicate(message))
                    {
                        return innerMessageHandler(message, cancellationToken);
                    }

                    return Task.CompletedTask;
                });
        }
    }
}