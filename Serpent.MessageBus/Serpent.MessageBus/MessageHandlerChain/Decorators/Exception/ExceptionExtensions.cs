// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.Exception;

    /// <summary>
    ///     The exception extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        private static readonly Task<bool> FalseTask = Task.FromResult(false);

        /// <summary>
        ///     Handles exceptions, optionally prevents them to propagate up the chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="exceptionHandlerFunc">
        ///     The method that handles the exception. If this method returns true, the exception
        ///     propagates further up the chain
        /// </param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new ExceptionDecorator<TMessageType>(currentHandler, (message, exception, token) => exceptionHandlerFunc(message, exception)));
        }

        /// <summary>
        ///     Handles exceptions, optionally prevents them to propagate up the chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="exceptionHandlerFunc">
        ///     The method that handles the exception. If this method returns true, the exception
        ///     propagates further up the chain
        /// </param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, CancellationToken, Task<bool>> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new ExceptionDecorator<TMessageType>(currentHandler, exceptionHandlerFunc));
        }

        /// <summary>
        ///     Handles exceptions
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="exceptionHandlerFunc">The method that handles the exception. </param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, Task> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new ExceptionDecorator<TMessageType>(
                    currentHandler,
                    async (message, exception, token) =>
                        {
                            await exceptionHandlerFunc(message, exception).ConfigureAwait(false);
                            return false;
                        }));
        }

        /// <summary>
        ///     Handles exceptions
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="exceptionHandlerFunc">The method that handles the exception. </param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, CancellationToken, Task> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new ExceptionDecorator<TMessageType>(
                    currentHandler,
                    async (message, exception, token) =>
                        {
                            await exceptionHandlerFunc(message, exception, token).ConfigureAwait(false);
                            return false;
                        }));
        }

        /// <summary>
        ///     Handles exceptions, optionally prevents them to propagate up the chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="exceptionHandlerFunc">
        ///     The method that handles the exception. If this method returns true, the exception
        ///     propagates further up the chain
        /// </param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, bool> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new ExceptionDecorator<TMessageType>(currentHandler, (message, exception, token) => Task.FromResult(exceptionHandlerFunc(message, exception))));
        }

        /// <summary>
        ///     Handles exceptions
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="exceptionHandlerAction">The method that handles the exception. </param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<TMessageType, Exception> exceptionHandlerAction)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new ExceptionDecorator<TMessageType>(
                    currentHandler,
                    (message, exception, token) =>
                        {
                            exceptionHandlerAction(message, exception);
                            return FalseTask;
                        }));
        }

        /// <summary>
        ///     Prevents the exception from propagating up the chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> IgnoreExceptions<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return async (message, token) =>
                            {
                                try
                                {
                                    await innerMessageHandler(message, token).ConfigureAwait(false);
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            };
                    });
        }
    }
}