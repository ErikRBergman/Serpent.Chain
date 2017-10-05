// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Filter;

    public static class FilterExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> beforeInvoke = null,
            Func<TMessageType, Task> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new FilterDecorator<TMessageType>(currentHandler, (msg, _) => beforeInvoke?.Invoke(msg), (msg, _) => afterInvoke?.Invoke(msg)));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task<bool>> beforeInvoke = null,
            Func<TMessageType, CancellationToken, Task> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(currentHandler => new FilterDecorator<TMessageType>(currentHandler, beforeInvoke, afterInvoke));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> beforeInvoke = null,
            Action<TMessageType> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new FilterDecorator<TMessageType>(
                    currentHandler,
                    (message, token) => Task.FromResult(beforeInvoke?.Invoke(message) ?? true),
                    (message, token) =>
                        {
                            afterInvoke?.Invoke(message);
                            return Task.CompletedTask;
                        }));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<TMessageType> beforeInvoke = null,
            Action<TMessageType> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new FilterDecorator<TMessageType>(
                    currentHandler,
                    (message, token) =>
                        {
                            beforeInvoke?.Invoke(message);
                            return Task.FromResult(true);
                        },
                    (message, token) =>
                        {
                            afterInvoke?.Invoke(message);
                            return Task.CompletedTask;
                        }));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Func<TMessageType, CancellationToken, Task>, Task> filterFunc)
        {
            return messageHandlerChainBuilder.Add(innerMessageHandler => { return (message, token) => filterFunc(message, token, innerMessageHandler); });
        }
    }
}