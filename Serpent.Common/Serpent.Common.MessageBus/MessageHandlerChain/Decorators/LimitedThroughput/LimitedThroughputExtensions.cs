// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.LimitedThroughput;

    /// <summary>
    /// The limited throughput decorator extensions
    /// </summary>
    public static class LimitedThroughputExtensions
    {
        /// <summary>
        /// Limits throughput to a fixed number of messages during a period of time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="maxMessagesPerPeriod">The maximum number of messages per period</param>
        /// <param name="periodSpan">The period span</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> LimitedThroughput<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxMessagesPerPeriod,
            TimeSpan? periodSpan = null)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new LimitedThroughputDecorator<TMessageType>(currentHandler, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)).HandleMessageAsync);
        }

        /// <summary>
        /// Limits throughput to a fixed number of messages during a period of time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="maxMessagesPerPeriod">The maximum number of messages per period</param>
        /// <param name="periodSpan">The period span</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> LimitedThroughputFireAndForget<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxMessagesPerPeriod,
            TimeSpan? periodSpan = null)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new LimitedThroughputFireAndForgetDecorator<TMessageType>(currentHandler, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)).HandleMessageAsync);
        }
    }
}