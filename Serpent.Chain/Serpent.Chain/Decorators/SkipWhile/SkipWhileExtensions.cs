// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading.Tasks;
    using Serpent.Chain.Decorators.SkipWhile;

    /// <summary>
    /// Provides extensions for the .SkipWhile decorator
    /// </summary>
    public static class SkipWhileExtensions
    {
        /// <summary>
        /// Skips messages while the predicate returns true, then passes all through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate to determine whether to skip initial messages</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> SkipWhile<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, bool> predicate)
        {
            return chainBuilder.AddDecorator(currentHandler => new SkipWhileDecorator<TMessageType>(currentHandler, predicate));
        }

        /// <summary>
        /// Skips messages while the predicate returns true, then passes all through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate to determine whether to skip initial messages</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> SkipWhile<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task<bool>> predicate)
        {
            return chainBuilder.AddDecorator(currentHandler => new SkipWhileAsyncDecorator<TMessageType>(currentHandler, predicate));
        }
    }
}