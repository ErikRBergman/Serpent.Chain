// ReSharper disable CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Retry;
    using Serpent.Chain.Models;

    /// <summary>
    ///     Provides extensions for more flexible retry configuration
    /// </summary>
    public static class RetryDecoratorBuilderExtensions
    {
        /// <summary>
        ///     Sets the maximum number of attempts to handle messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="count">The maximum number of messages</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> MaximumNumberOfAttempts<TMessageType>(this IRetryDecoratorBuilder<TMessageType> builder, int count)
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
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> handlerFailedFunc)
        {
            builder.HandlerFailedFunc = handlerFailedFunc ?? throw new ArgumentNullException(nameof(handlerFailedFunc));
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(this IRetryDecoratorBuilder<TMessageType> builder, Action handlerFailedFunc)
        {
            if (handlerFailedFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedFunc));
            }

            return builder.OnFail(
                attempt =>
                    {
                        handlerFailedFunc();
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<FailedMessageHandlingAttempt<TMessageType>, Task> handlerFailedFunc)
        {
            if (handlerFailedFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedFunc));
            }

            builder.HandlerFailedFunc = (message, exception, attempt, maxAttempts, delay, token) => handlerFailedFunc(
                new FailedMessageHandlingAttempt<TMessageType>
                    {
                        AttemptNumber = attempt,
                        Message = message,
                        CancellationToken = token,
                        Delay = delay,
                        Exception = exception,
                        MaximumNumberOfAttemps = maxAttempts
                    });

            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedAction">The method to call when the message handler fails</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Action<TMessageType, Exception, int, int, TimeSpan, CancellationToken> handlerFailedAction)
        {
            if (handlerFailedAction == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedAction));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Action<TMessageType, Exception, int, int, TimeSpan> handlerFailedAction)
        {
            if (handlerFailedAction == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedAction));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Action<TMessageType, Exception, int, int> handlerFailedAction)
        {
            if (handlerFailedAction == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedAction));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, TimeSpan, Task> handlerFailedFunc)
        {
            if (handlerFailedFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedFunc));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, Task> handlerFailedFunc)
        {
            if (handlerFailedFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedFunc));
            }

            builder.HandlerFailedFunc = (message, exception, attempt, maxAttempts, delay, cancellationToken) => handlerFailedFunc(message, exception, attempt, maxAttempts);
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler succeeds
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerSucceededFunc">The method to call when a message handler succeeds</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnSuccess<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, int, int, TimeSpan, Task> handlerSucceededFunc)
        {
            if (handlerSucceededFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerSucceededFunc));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> OnSuccess<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<MessageHandlingAttempt<TMessageType>, Task> handlerSucceededFunc)
        {
            if (handlerSucceededFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerSucceededFunc));
            }

            builder.HandlerSucceededFunc = (msg, attempt, maxAttempts, delay) => handlerSucceededFunc(
                new MessageHandlingAttempt<TMessageType>
                    {
                        Message = msg,
                        AttemptNumber = attempt,
                        MaximumNumberOfAttemps = maxAttempts,
                        Delay = delay
                    });
            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler succeeds
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerSucceededFunc">The method to call when a message handler succeeds</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnSuccess<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, int, int, Task> handlerSucceededFunc)
        {
            if (handlerSucceededFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerSucceededFunc));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> OnSuccess<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Action<TMessageType, int, int, TimeSpan> handlerSucceededAction)
        {
            if (handlerSucceededAction == null)
            {
                throw new ArgumentNullException(nameof(handlerSucceededAction));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> OnSuccess<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Action<TMessageType, int, int> handlerSucceededAction)
        {
            if (handlerSucceededAction == null)
            {
                throw new ArgumentNullException(nameof(handlerSucceededAction));
            }

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
        public static IRetryDecoratorBuilder<TMessageType> RetryDelay<TMessageType>(this IRetryDecoratorBuilder<TMessageType> builder, TimeSpan delay)
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
        public static IRetryDecoratorBuilder<TMessageType> RetryHandler<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            IMessageHandlerRetry<TMessageType> retryHandler)
        {
            return builder.OnFail(retryHandler.HandleRetryAsync).OnSuccess(retryHandler.MessageHandledSuccessfullyAsync);
        }
    }
}