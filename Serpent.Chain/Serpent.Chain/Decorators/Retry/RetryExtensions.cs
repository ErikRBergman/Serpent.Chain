// ReSharper disable once CheckNamespace
// ReSharper disable StyleCop.SA1118

namespace Serpent.Chain
{
    using System;

    using Serpent.Chain.Decorators.Retry;

    /// <summary>
    ///     The .Retry() decorator extensions
    /// </summary>
    public static class RetryExtensions
    {
        /// <summary>
        ///     Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder.</param>
        /// <param name="maxNumberOfAttempts">The maximum number of attempts to try.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <returns>The MHC builder</returns>
        public static IChainBuilder<TMessageType> Retry<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            int maxNumberOfAttempts,
            TimeSpan retryDelay)
        {
            var builder = new RetryDecoratorBuilder<TMessageType>().MaximumNumberOfAttempts(maxNumberOfAttempts).RetryDelays(retryDelay);
            return chainBuilder.AddDecorator(currentHandler => new RetryDecorator<TMessageType>(currentHandler, builder).HandleMessageAsync);
        }

        /// <summary>
        ///     Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder.</param>
        /// <param name="configureRetry">Metod called to configure the retry</param>
        /// <returns>The MHC builder</returns>
        public static IChainBuilder<TMessageType> Retry<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Action<IRetryDecoratorBuilder<TMessageType>> configureRetry)
        {
            if (configureRetry == null)
            {
                throw new ArgumentNullException(nameof(configureRetry));
            }

            var builder = new RetryDecoratorBuilder<TMessageType>();
            configureRetry(builder);

            return chainBuilder.AddDecorator(currentHandler => new RetryDecorator<TMessageType>(currentHandler, builder).HandleMessageAsync);
        }
    }
}