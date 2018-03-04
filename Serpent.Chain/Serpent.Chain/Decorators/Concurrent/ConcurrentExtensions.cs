// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using Serpent.Chain.Decorators.Concurrent;

    /// <summary>
    /// The concurrent decorator extensions
    /// </summary>
    public static class ConcurrentExtensions
    {
        /// <summary>
        /// Parallelize the message throughput to X concurrent messages.
        /// </summary>
        /// <typeparam name="TMessageType">The message bus message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="maxNumberOfConcurrentMessages">The maximum number of concurrent messages being handled (the level of parallelism)</param>
        /// <returns>The message handler chain builder used to stack more decorators</returns>
        public static IChainBuilder<TMessageType> Concurrent<TMessageType>(this IChainBuilder<TMessageType> chainBuilder, int maxNumberOfConcurrentMessages)
        {
            return chainBuilder.AddDecorator(nextHandler => new ConcurrentDecorator<TMessageType>(nextHandler, maxNumberOfConcurrentMessages));
        }

        /// <summary>
        /// Parallelize the message throughput to X concurrent messages, dropping/breaking the feedback chain.
        /// </summary>
        /// <typeparam name="TMessageType">The message bus message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="maxNumberOfConcurrentMessages">The maximum number of concurrent messages being handled (the level of parallelism)</param>
        /// <returns>The message handler chain builder used to stack more decorators</returns>
        public static IChainBuilder<TMessageType> ConcurrentFireAndForget<TMessageType>(this IChainBuilder<TMessageType> chainBuilder, int maxNumberOfConcurrentMessages)
        {
            return chainBuilder.AddDecorator(nextHandler => new ConcurrentFireAndForgetDecorator<TMessageType>(nextHandler, maxNumberOfConcurrentMessages));
        }
    }
}