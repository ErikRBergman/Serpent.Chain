// ReSharper disable once CheckNamespace
// ReSharper disable StyleCop.SA1118

namespace Serpent.MessageHandlerChain
{
    using System;

    using Serpent.MessageHandlerChain.Decorators.Retry;

    /// <summary>
    ///     The .Retry() decorator extensions
    /// </summary>
    public static class RetryExtensions
    {
        /// <summary>
        ///     Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder.</param>
        /// <param name="maxNumberOfAttempts">The maximum number of attempts to try.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <returns>The MHC builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Retry<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxNumberOfAttempts,
            TimeSpan retryDelay)
        {
            var builder = new RetryDecoratorBuilder<TMessageType>().MaximumNumberOfAttempts(maxNumberOfAttempts).RetryDelay(retryDelay);
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new RetryDecorator<TMessageType>(currentHandler, builder).HandleMessageAsync);
        }

        /// <summary>
        ///     Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder.</param>
        /// <param name="configureRetry">Metod called to configure the retry</param>
        /// <returns>The MHC builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Retry<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<IRetryDecoratorBuilder<TMessageType>> configureRetry)
        {
            if (configureRetry == null)
            {
                throw new ArgumentNullException(nameof(configureRetry));
            }

            var builder = new RetryDecoratorBuilder<TMessageType>();
            configureRetry(builder);

            return messageHandlerChainBuilder.AddDecorator(currentHandler => new RetryDecorator<TMessageType>(currentHandler, builder).HandleMessageAsync);
        }
    }
}