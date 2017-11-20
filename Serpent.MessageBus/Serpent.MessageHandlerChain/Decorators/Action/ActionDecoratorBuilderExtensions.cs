// ReSharper disable CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Helpers;

    /// <summary>
    ///     Provides extensions for configuring the append decorator
    /// </summary>
    public static class ActionDecoratorBuilderExtensions
    {
        /// <summary>
        ///     Invokes a method before the message is handled
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">
        ///     The action decorator builder
        /// </param>
        /// <param name="beforeFunc">
        ///     The method
        /// </param>
        /// <returns>
        ///     A builder
        /// </returns>
        public static IActionDecoratorBuilder<TMessageType> Before<TMessageType>(this IActionDecoratorBuilder<TMessageType> builder, Func<TMessageType, Task> beforeFunc)
        {
            return builder.Before((message, token) => beforeFunc(message));
        }

        /// <summary>
        ///     Invokes a method before the message is handled
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">
        ///     The action decorator builder
        /// </param>
        /// <param name="beforeFunc">
        ///     The method
        /// </param>
        /// <returns>
        ///     A builder
        /// </returns>
        public static IActionDecoratorBuilder<TMessageType> Before<TMessageType>(this IActionDecoratorBuilder<TMessageType> builder, Action<TMessageType> beforeFunc)
        {
            return builder.Before(
                (message, token) =>
                    {
                        beforeFunc(message);
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        ///     Invokes a method when all handling at this point is done
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="finallyFunc">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> Finally<TMessageType>(this IActionDecoratorBuilder<TMessageType> builder, Func<TMessageType, Task> finallyFunc)
        {
            return builder.Finally((message, token, exception) => finallyFunc(message));
        }

        /// <summary>
        ///     Invokes a method when all handling at this point is done
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="finallyAction">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> Finally<TMessageType>(
            this IActionDecoratorBuilder<TMessageType> builder,
            Action<TMessageType, CancellationToken, Exception> finallyAction)
        {
            return builder.Finally(
                (message, token, exception) =>
                    {
                        finallyAction(message, token, exception);
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        ///     Invokes a method when all handling at this point is done
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="finallyAction">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> Finally<TMessageType>(this IActionDecoratorBuilder<TMessageType> builder, Action<TMessageType> finallyAction)
        {
            return builder.Finally(
                (message, token, exception) =>
                    {
                        finallyAction(message);
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        ///     Invokes a method when a message is cancelled
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="onCancelFunc">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> OnCancel<TMessageType>(this IActionDecoratorBuilder<TMessageType> builder, Action<TMessageType> onCancelFunc)
        {
            return builder.OnCancel(
                message =>
                    {
                        onCancelFunc(message);
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        ///     Invokes a method when a message is cancelled
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="onCancelFunc">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> OnCancel<TMessageType>(this IActionDecoratorBuilder<TMessageType> builder, Func<TMessageType, Task> onCancelFunc)
        {
            return builder.Before((message, token) => onCancelFunc(message));
        }

        /// <summary>
        ///     Invokes a method when a message handler throws an exception
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="onExceptionFunc">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> OnException<TMessageType>(
            this IActionDecoratorBuilder<TMessageType> builder,
            Action<TMessageType, Exception> onExceptionFunc)
        {
            return builder.OnException(
                (message, exception) =>
                    {
                        onExceptionFunc(message, exception);
                        return TaskHelper.TrueTask;
                    });
        }

                /// <summary>
        ///     Invokes a method when a message handler throws an exception
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="onExceptionFunc">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> OnException<TMessageType>(
            this IActionDecoratorBuilder<TMessageType> builder,
            Action<TMessageType> onExceptionFunc)
        {
            return builder.OnException(
                (message, exception) =>
                    {
                        onExceptionFunc(message);
                        return TaskHelper.TrueTask;
                    });
        }

        /// <summary>
        ///     Invokes a method when a message is handled successfully
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="builder">The action builder</param>
        /// <param name="onSuccessAction">The method</param>
        /// <returns>A builder</returns>
        public static IActionDecoratorBuilder<TMessageType> OnSuccess<TMessageType>(this IActionDecoratorBuilder<TMessageType> builder, Action<TMessageType> onSuccessAction)
        {
            return builder.OnSuccess(
                message =>
                    {
                        onSuccessAction(message);
                        return Task.CompletedTask;
                    });
        }
    }
}