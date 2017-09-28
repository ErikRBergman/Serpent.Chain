// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

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
                    message =>
                        {
                            var result = beforeInvoke == null || beforeInvoke(message);
                            return Task.FromResult(result);
                        },
                    message =>
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
                    message =>
                        {
                            beforeInvoke?.Invoke(message);
                            return Task.FromResult(true);
                        },
                    message =>
                        {
                            afterInvoke?.Invoke(message);
                            return Task.CompletedTask;
                        }));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Func<TMessageType, Task>, Task> filterFunc)
        {
            return messageHandlerChainBuilder.Add(innerMessageHandler => { return message => filterFunc(message, innerMessageHandler); });
        }
    }
}