// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class ExceptionExtensions
    {
        private static readonly Task<bool> FalseTask = Task.FromResult(false);

        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new ExceptionDecorator<TMessageType>(currentHandler, exceptionHandlerFunc));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, CancellationToken, Task<bool>> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new ExceptionDecorator<TMessageType>(currentHandler, exceptionHandlerFunc));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, Task> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new ExceptionDecorator<TMessageType>(
                    currentHandler,
                    async (message, exception) =>
                        {
                            await exceptionHandlerFunc(message, exception).ConfigureAwait(false);
                            return false;
                        }));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, CancellationToken, Task> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new ExceptionDecorator<TMessageType>(
                    currentHandler,
                    async (message, exception, token) =>
                        {
                            await exceptionHandlerFunc(message, exception, token).ConfigureAwait(false);
                            return false;
                        }));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Exception, bool> exceptionHandlerFunc)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new ExceptionDecorator<TMessageType>(
                    currentHandler,
                    (message, exception) => Task.FromResult(exceptionHandlerFunc(message, exception))));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Exception<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<TMessageType, Exception> exceptionHandlerAction)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new ExceptionDecorator<TMessageType>(
                    currentHandler,
                    (message, exception) =>
                        {
                            exceptionHandlerAction(message, exception);
                            return FalseTask;
                        }));
        }
    }
}