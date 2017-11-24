// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.TakeWhile;

    /// <summary>
    /// Provides extension for the .TakeWhile() decorator
    /// </summary>
    public static class TakeWhileExtensions
    {
        /// <summary>
        /// Handle messages as long as the predicate returns true, then unsubscribes
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate used to determine if messages can continue to flow</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> TakeWhile<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, bool> predicate)
        {
            return chainBuilder.AddDecorator((currentHandler, services) => new TakeWhileDecorator<TMessageType>(new TakeWhileDecoratorConfiguration<TMessageType>
            {
                HandlerFunc = currentHandler,
                Predicate = predicate,
                Services = services
            }));
        }

        /// <summary>
        /// Handle messages as long as the predicate returns true, then unsubscribes
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate used to determine if messages can continue to flow</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> TakeWhile<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task<bool>> predicate)
        {
            return chainBuilder.AddDecorator((currentHandler, services) => new TakeWhileAsyncDecorator<TMessageType>(new TakeWhileAsyncDecoratorConfiguration<TMessageType>
            {
                HandlerFunc = currentHandler,
                Predicate = predicate,
                Services = services
            }));
        }
    }
}