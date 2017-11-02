// ReSharper disable CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.Retry;

    /// <summary>
    ///     Provides extensions for more flexible retry configuration
    /// </summary>
    public static class RetryBuilderExtensions
    {
        /// <summary>
        ///     Sets the maximum number of attempts to handle messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="count">The maximum number of messages</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> MaximumNumberOfAttempts<TMessageType>(this IRetryBuilder<TMessageType> builder, int count)
        {
            builder.MaximumNumberOfAttempts = count;
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> handlerFailedFunc)
        {
            builder.HandlerFailedFunc = handlerFailedFunc;
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedAction">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryBuilder<TMessageType> builder,
            Action<TMessageType, Exception, int, int, TimeSpan, CancellationToken> handlerFailedAction)
        {
            builder.HandlerFailedFunc = (msg, exception, attempts, maxAttempts, delay, token) =>
                {
                    handlerFailedAction(msg, exception, attempts, maxAttempts, delay, token);
                    return Task.CompletedTask;
                };

            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedAction">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryBuilder<TMessageType> builder,
            Action<TMessageType, Exception, int, int, TimeSpan> handlerFailedAction)
        {
            builder.HandlerFailedFunc = (msg, exception, attempts, maxAttempts, delay, token) =>
                {
                    handlerFailedAction(msg, exception, attempts, maxAttempts, delay);
                    return Task.CompletedTask;
                };

            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedAction">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnFail<TMessageType>(this IRetryBuilder<TMessageType> builder, Action<TMessageType, Exception, int, int> handlerFailedAction)
        {
            builder.HandlerFailedFunc = (msg, exception, attempts, maxAttempts, delay, token) =>
                {
                    handlerFailedAction(msg, exception, attempts, maxAttempts);
                    return Task.CompletedTask;
                };

            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, TimeSpan, Task> handlerFailedFunc)
        {
            builder.HandlerFailedFunc = (msg, exception, attempt, maxAttempts, delay, token) => handlerFailedFunc(msg, exception, attempt, maxAttempts, delay);
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnFail<TMessageType>(this IRetryBuilder<TMessageType> builder, Func<TMessageType, Exception, int, int, Task> handlerFailedFunc)
        {
            builder.HandlerFailedFunc = (msg, exception, attempt, maxAttempts, delay, token) => handlerFailedFunc(msg, exception, attempt, maxAttempts);
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler succeeds
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerSucceededFunc">The method to call when a message handler succeeds</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnSuccess<TMessageType>(
            this IRetryBuilder<TMessageType> builder,
            Func<TMessageType, int, int, TimeSpan, Task> handlerSucceededFunc)
        {
            builder.HandlerSucceededFunc = handlerSucceededFunc;
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler succeeds
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerSucceededFunc">The method to call when a message handler succeeds</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnSuccess<TMessageType>(this IRetryBuilder<TMessageType> builder, Func<TMessageType, int, int, Task> handlerSucceededFunc)
        {
            builder.HandlerSucceededFunc = (msg, attempt, maxAttempts, delay) => handlerSucceededFunc(msg, attempt, maxAttempts);
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler succeeds
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerSucceededAction">The method to call when a message handler succeeds</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnSuccess<TMessageType>(this IRetryBuilder<TMessageType> builder, Action<TMessageType, int, int, TimeSpan> handlerSucceededAction)
        {
            builder.HandlerSucceededFunc = (msg, attempt, maxAttempts, delay) =>
                {
                    handlerSucceededAction(msg, attempt, maxAttempts, delay);
                    return Task.CompletedTask;
                };
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler succeeds
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerSucceededAction">The method to call when a message handler succeeds</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> OnSuccess<TMessageType>(this IRetryBuilder<TMessageType> builder, Action<TMessageType, int, int> handlerSucceededAction)
        {
            builder.HandlerSucceededFunc = (msg, attempt, maxAttempts, delay) =>
                {
                    handlerSucceededAction(msg, attempt, maxAttempts);
                    return Task.CompletedTask;
                };
            return builder;
        }

        /// <summary>
        ///     Sets the delay between attempts to handle messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="delay">The delay between attempts</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> RetryDelay<TMessageType>(this IRetryBuilder<TMessageType> builder, TimeSpan delay)
        {
            builder.RetryDelay = delay;
            return builder;
        }

        /// <summary>
        ///     Configures a retry handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="retryHandler">The retry handler</param>
        /// <returns>A retry builder</returns>
        public static IRetryBuilder<TMessageType> RetryHandler<TMessageType>(this IRetryBuilder<TMessageType> builder, IMessageHandlerRetry<TMessageType> retryHandler)
        {
            return builder.OnFail(retryHandler.HandleRetryAsync).OnSuccess(retryHandler.MessageHandledSuccessfullyAsync);
        }
    }
}