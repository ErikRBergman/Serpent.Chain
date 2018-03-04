// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;

    using Serpent.Chain.Decorators.Delay;

    /// <summary>
    /// The delay decorator extensions
    /// </summary>
    public static class DelayExtensions
    {
        /// <summary>
        /// Delay handling each message by a specified time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="timeToWait">The timespan to await</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> Delay<TMessageType>(this IChainBuilder<TMessageType> chainBuilder, TimeSpan timeToWait)
        {
            return chainBuilder.AddDecorator(nextHandler => new DelayDecorator<TMessageType>(nextHandler, timeToWait).HandleMessageAsync);
        }

        /// <summary>
        /// Delay handling each message by a specified time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="timeInMilliseconds">The time to await in milliseconds</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> Delay<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            int timeInMilliseconds)
        {
            return chainBuilder.AddDecorator(
                nextHandler => new DelayDecorator<TMessageType>(nextHandler, TimeSpan.FromMilliseconds(timeInMilliseconds)).HandleMessageAsync);
        }
    }
}