// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry;

    public static class RetryExtensions
    {
        /// <summary>
        /// Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder.</param>
        /// <param name="maxNumberOfAttempts">The maximum number of attempts to try.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <param name="exceptionFunc">Optional function called for each time the message handler throws an exception.</param>
        /// <param name="successFunc">Option function called after the messgae handler has succeeded.</param>
        /// <returns>The MHC builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Retry<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxNumberOfAttempts,
            TimeSpan retryDelay,
            Func<TMessageType, Exception, int, int, Task> exceptionFunc = null,
            Func<TMessageType, Task> successFunc = null)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new RetryDecorator<TMessageType>(currentHandler, maxNumberOfAttempts, retryDelay, exceptionFunc, successFunc).HandleMessageAsync);
        }

        /// <summary>
        /// Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder.</param>
        /// <param name="maxNumberOfAttempts">The maximum number of attempts to try.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <param name="exceptionAction">Optional function called for each time the message handler throws an exception.</param>
        /// <param name="successAction">Option function called after the messgae handler has succeeded.</param>
        /// <returns>The subscription builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Retry<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxNumberOfAttempts,
            TimeSpan retryDelay,
            Action<TMessageType, Exception, int, int> exceptionAction,
            Action<TMessageType> successAction = null)
        {
            Func<TMessageType, Exception, int, int, Task> exceptionFunc = null;
            Func<TMessageType, Task> successFunc = null;

            if (exceptionAction != null)
            {
                exceptionFunc = (message, exception, attempt, maxAttempts) =>
                    {
                        exceptionAction(message, exception, attempt, maxAttempts);
                        return Task.CompletedTask;
                    };
            }

            if (successAction != null)
            {
                successFunc = message =>
                    {
                        successAction(message);
                        return Task.CompletedTask;
                    };
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new RetryDecorator<TMessageType>(currentHandler, maxNumberOfAttempts, retryDelay, exceptionFunc, successFunc).HandleMessageAsync);
        }
    }
}