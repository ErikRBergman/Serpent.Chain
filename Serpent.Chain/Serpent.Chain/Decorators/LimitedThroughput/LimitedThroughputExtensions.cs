// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;

    using Serpent.Chain.Decorators.LimitedThroughput;

    /// <summary>
    /// The limited throughput decorator extensions
    /// </summary>
    public static class LimitedThroughputExtensions
    {
        /// <summary>
        /// Limits throughput to a fixed number of messages during a period of time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="maxMessagesPerPeriod">The maximum number of messages per period</param>
        /// <param name="periodSpan">The period span</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> LimitedThroughput<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            int maxMessagesPerPeriod,
            TimeSpan? periodSpan = null)
        {
            return chainBuilder.AddDecorator(
                currentHandler => new LimitedThroughputDecorator<TMessageType>(currentHandler, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)));
        }

        /// <summary>
        /// Limits throughput to a fixed number of messages during a period of time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="maxMessagesPerPeriod">The maximum number of messages per period</param>
        /// <param name="periodSpan">The period span</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> LimitedThroughputFireAndForget<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            int maxMessagesPerPeriod,
            TimeSpan? periodSpan = null)
        {
            return chainBuilder.AddDecorator(
                currentHandler => new LimitedThroughputFireAndForgetDecorator<TMessageType>(currentHandler, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)));
        }
    }
}