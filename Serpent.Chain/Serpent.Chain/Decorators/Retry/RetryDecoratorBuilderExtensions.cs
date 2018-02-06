// ReSharper disable CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Retry;
    using Serpent.Chain.Helpers;
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
        /// <param name="handlerFailedFunc">The method to call when the message handler fails. Return false to cancel further retry attempts.</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task<bool>> handlerFailedFunc)
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
                        return TaskHelper.TrueTask;
                    });
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails. Return false to cancel further retry attempts.</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(this IRetryDecoratorBuilder<TMessageType> builder, Func<bool> handlerFailedFunc)
        {
            if (handlerFailedFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerFailedFunc));
            }

            return builder.OnFail(attempt => TaskHelper.FromResult(handlerFailedFunc()));
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails. Return false to cancel further retry attempts.</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<FailedMessageHandlingAttempt<TMessageType>, Task<bool>> handlerFailedFunc)
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
        ///     Retries only when the predicate function returns true
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="predicate">The predicate .</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> Where<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<FailedMessageHandlingAttempt<TMessageType>, bool> predicate)
        {
            builder.WherePredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

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
                    return TaskHelper.TrueTask;
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
                    return TaskHelper.TrueTask;
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
                    return TaskHelper.TrueTask;
                };

            return builder;
        }

        /// <summary>
        ///     Sets the method called when a message handler fails (throws an exception)
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="handlerFailedFunc">The method to call when the message handler fails. Return false to cancel further retry attempts.</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, TimeSpan, Task<bool>> handlerFailedFunc)
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
        /// <param name="handlerFailedFunc">The method to call when the message handler fails. Return false to cancel further retry attempts.</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> OnFail<TMessageType>(
            this IRetryDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, Exception, int, int, Task<bool>> handlerFailedFunc)
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
            Func<TMessageType, int, int, Task> handlerSucceededFunc)
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

            builder.HandlerSucceededFunc = (msg, attempt, maxAttempts) => handlerSucceededFunc(
                new MessageHandlingAttempt<TMessageType>
                {
                    Message = msg,
                    AttemptNumber = attempt,
                    MaximumNumberOfAttemps = maxAttempts
                });
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

            builder.HandlerSucceededFunc = (msg, attempt, maxAttempts) =>
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
        /// <param name="delays">The delay(s) between the attempts. The first delay is after the first attempt, the second after the second and so on. The last delay is used for all attempts after that.</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> RetryDelays<TMessageType>(this IRetryDecoratorBuilder<TMessageType> builder, params TimeSpan[] delays)
        {
            builder.RetryDelays = delays;
            return builder;
        }

        /// <summary>
        ///     Sets the delay between attempts to handle messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="builder">The retry builder</param>
        /// <param name="delays">The delay(s) between the attempts. The first delay is after the first attempt, the second after the second and so on. The last delay is used for all attempts after that.</param>
        /// <returns>A retry builder</returns>
        public static IRetryDecoratorBuilder<TMessageType> RetryDelays<TMessageType>(this IRetryDecoratorBuilder<TMessageType> builder, IEnumerable<TimeSpan> delays)
        {
            builder.RetryDelays = delays;
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
            builder.RetryDelays = new[] { delay };
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