// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;

    using Serpent.Chain.Decorators.Action;

    /// <summary>
    ///     The filter extensions
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        ///     Adds an action decorator
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="configure">Sets up the events to handle</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> Action<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Action<IActionDecoratorBuilder<TMessageType>> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var builder = new ActionDecoratorBuilder<TMessageType>();
            configure(builder);
            return chainBuilder.AddDecorator(builder);
        }
    }
}