// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extensions for the .Where() decorator
    /// </summary>
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

            return messageHandlerChainBuilder.AddDecorator(innerMessageHandler =>
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

            return messageHandlerChainBuilder.AddDecorator(WhereDecoratorFunc(predicate));
        }

        internal static Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> WhereDecoratorFunc<TMessageType>(Func<TMessageType, bool> predicate)
        {
            return innerMessageHandler => (message, cancellationToken) =>
                {
                    if (predicate(message))
                    {
                        return innerMessageHandler(message, cancellationToken);
                    }

                    return Task.CompletedTask;
                };
        }
    }
}