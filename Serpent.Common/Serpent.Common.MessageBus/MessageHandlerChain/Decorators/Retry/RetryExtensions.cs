// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry;

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
            return messageHandlerChainBuilder.Add(currentHandler => new RetryDecorator<TMessageType>(currentHandler, maxNumberOfAttempts, retryDelay).HandleMessageAsync);
        }

        /// <summary>
        ///     Retry the message dispatch
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
            Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> exceptionFunc = null,
            Func<TMessageType, int, int, TimeSpan, Task> successFunc = null)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new RetryDecorator<TMessageType>(currentHandler, maxNumberOfAttempts, retryDelay, exceptionFunc, successFunc).HandleMessageAsync);
        }

        /// <summary>
        ///     Retry the message dispatch
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
            Func<TMessageType, int, int, Task> successFunc = null)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new RetryDecorator<TMessageType>(
                        currentHandler,
                        maxNumberOfAttempts,
                        retryDelay,
                        exceptionFunc == null
                            ? (Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task>)null
                            : (msg, exception, attempt, maxAttempts, delay, token) => exceptionFunc(msg, exception, attempt, maxAttempts),
                        successFunc == null
                            ? (Func<TMessageType, int, int, TimeSpan, Task>)null
                            : (msg, attempt, maxAttempts, delay) => successFunc?.Invoke(msg, attempt, maxAttempts))
                    .HandleMessageAsync);
        }

        /// <summary>
        ///     Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder.</param>
        /// <param name="maxNumberOfAttempts">The maximum number of attempts to try.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <param name="exceptionAction">Optional function called for each time the message handler throws an exception.</param>
        /// <param name="successAction">Option function called after the messgae handler has succeeded.</param>
        /// <returns>The MHC builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Retry<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxNumberOfAttempts,
            TimeSpan retryDelay,
            Action<TMessageType, Exception, int, int> exceptionAction = null,
            Action<TMessageType, int, int> successAction = null)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new RetryDecorator<TMessageType>(
                    currentHandler,
                    maxNumberOfAttempts,
                    retryDelay,
                    exceptionAction == null
                        ? (Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task>)null
                        :
                    (msg, exception, attempt, maxAttempts, delay, token) =>
                        {
                            exceptionAction?.Invoke(msg, exception, attempt, maxAttempts);
                            return Task.CompletedTask;
                        },
                    successAction == null
                        ? (Func<TMessageType, int, int, TimeSpan, Task>)null
                        : (msg, attempt, maxAttempts, delay) =>
                        {
                            successAction(msg, attempt, maxAttempts);
                            return Task.CompletedTask;
                        }).HandleMessageAsync);
        }

        /// <summary>
        ///     Retry the message dispatch
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
            Action<TMessageType, Exception, int, int, TimeSpan, CancellationToken> exceptionAction,
            Action<TMessageType, int, int, TimeSpan> successAction = null)
        {
            Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> exceptionFunc = null;
            Func<TMessageType, int, int, TimeSpan, Task> successFunc = null;

            if (exceptionAction != null)
            {
                exceptionFunc = (message, exception, attempt, maxAttempts, innerRetryDelay, token) =>
                    {
                        exceptionAction(message, exception, attempt, maxAttempts, innerRetryDelay, token);
                        return Task.CompletedTask;
                    };
            }

            if (successAction != null)
            {
                successFunc = (message, attempt, maxAttempts, innerRetryDelay) =>
                    {
                        successAction(message, attempt, maxAttempts, innerRetryDelay);
                        return Task.CompletedTask;
                    };
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new RetryDecorator<TMessageType>(currentHandler, maxNumberOfAttempts, retryDelay, exceptionFunc, successFunc).HandleMessageAsync);
        }

        /// <summary>
        ///     Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     Message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The message handler chain builder.
        /// </param>
        /// <param name="maxNumberOfAttempts">
        ///     The maximum number of attempts to try.
        /// </param>
        /// <param name="retryDelay">
        ///     The delay between retries.
        /// </param>
        /// <param name="retryHandler">
        ///     The retry Handler.
        /// </param>
        /// <returns>
        ///     The subscription builder
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> Retry<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxNumberOfAttempts,
            TimeSpan retryDelay,
            IMessageHandlerRetry<TMessageType> retryHandler)
        {
            return messageHandlerChainBuilder.Retry(maxNumberOfAttempts, retryDelay, retryHandler.HandleRetryAsync, retryHandler.MessageHandledSuccessfullyAsync);
        }
    }
}