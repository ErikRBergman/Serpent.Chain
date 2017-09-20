// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class FilterExtensions
    {
        public static SubscriptionBuilder<TMessageType> Filter<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            Func<TMessageType, Task<bool>> beforeInvoke = null,
            Func<TMessageType, Task> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return subscriptionBuilder;
            }

            return subscriptionBuilder.Add(currentHandler => new FilterSubscription<TMessageType>(currentHandler, beforeInvoke, afterInvoke));
        }

        public static SubscriptionBuilder<TMessageType> Filter<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            Func<TMessageType, bool> beforeInvoke = null,
            Action<TMessageType> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return subscriptionBuilder;
            }

            return subscriptionBuilder.Add(
                currentHandler => new FilterSubscription<TMessageType>(
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

        public static SubscriptionBuilder<TMessageType> Filter<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            Action<TMessageType> beforeInvoke = null,
            Action<TMessageType> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return subscriptionBuilder;
            }

            return subscriptionBuilder.Add(
                currentHandler => new FilterSubscription<TMessageType>(
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

        // public static IMessageBusSubscription CreateFireAndForgetSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc)
        // {
        // return messageBus.Subscribe(new FireAndForgetSubscription<TMessageType>(handlerFunc).HandleMessageAsync);
        // }

        // public static IMessageBusSubscription CreateFireAndForgetSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, IMessageHandler<TMessageType> handler)
        // {
        // return messageBus.Subscribe(new FireAndForgetSubscription<TMessageType>(handler.HandleMessageAsync).HandleMessageAsync);
        // }

        // public static IMessageBusSubscription CreateFireAndForgetSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription)
        // {
        // return messageBus.Subscribe(new FireAndForgetSubscription<TMessageType>(innerSubscription).HandleMessageAsync);
        // }
    }
}