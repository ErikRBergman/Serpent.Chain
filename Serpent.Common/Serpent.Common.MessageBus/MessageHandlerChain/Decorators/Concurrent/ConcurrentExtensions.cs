// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent;

    public static class ConcurrentExtensions
    {
        /// <summary>
        /// Parallelize the message throughput to X concurrent messages.
        /// </summary>
        /// <typeparam name="TMessageType">The message bus message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="maxNumberOfConcurrentMessages">The maximum number of concurrent messages being handled (the level of parallelism)</param>
        /// <returns>The message handler chain builder used to stack more decorators</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Concurrent<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, int maxNumberOfConcurrentMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new ConcurrentDecorator<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }

        /// <summary>
        /// Parallelize the message throughput to X concurrent messages, dropping/breaking the feedback chain.
        /// </summary>
        /// <typeparam name="TMessageType">The message bus message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="maxNumberOfConcurrentMessages">The maximum number of concurrent messages being handled (the level of parallelism)</param>
        /// <returns>The message handler chain builder used to stack more decorators</returns>
        public static IMessageHandlerChainBuilder<TMessageType> ConcurrentFireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, int maxNumberOfConcurrentMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new ConcurrentFireAndForgetDecorator<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }
    }
}