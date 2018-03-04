// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.First;

    /// <summary>
    /// The first decorator extensions
    /// </summary>
    public static class FirstExtensions
    {
        /// <summary>
        /// Handle only the first message where the predicate returns true, then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> First<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, bool> predicate)
        {
            return chainBuilder.Decorate((nextHandler, subscriptionServices) => new FirstDecorator<TMessageType>(nextHandler, predicate, subscriptionServices).HandleMessageAsync);
        }

        /// <summary>
        /// Handle only the first message then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> First<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.Decorate((nextHandler, subscriptionServices) => new FirstDecorator<TMessageType>(nextHandler, msg => true, subscriptionServices).HandleMessageAsync);
        }

        /// <summary>
        /// Handle only the first message where the predicate returns true, then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="asyncPredicate">The predicate</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> First<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task<bool>> asyncPredicate)
        {
            if (asyncPredicate == null)
            {
                throw new ArgumentNullException(nameof(asyncPredicate));
            }

            return chainBuilder.Decorate((nextHandler, subscriptionServices) => new FirstAsyncDecorator<TMessageType>(nextHandler, (message, token) => asyncPredicate(message), subscriptionServices).HandleMessageAsync);
        }

        /// <summary>
        /// Handle only the first message where the predicate returns true, then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="asyncPredicate">The predicate</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> First<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate)
        {
            return chainBuilder.Decorate((nextHandler, subscriptionServices) => new FirstAsyncDecorator<TMessageType>(nextHandler, asyncPredicate, subscriptionServices).HandleMessageAsync);
        }
    }
}