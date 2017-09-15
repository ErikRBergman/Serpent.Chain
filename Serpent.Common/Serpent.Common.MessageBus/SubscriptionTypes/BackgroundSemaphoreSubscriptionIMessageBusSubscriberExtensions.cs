 // ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class BackgroundSemaphoreSubscriptionIMessageBusSubscriberExtensions
    {
        public static SubscriptionWrapper<TMessageType> CreateBackgroundSemaphoreSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc, int concurrencyLevel = -1)
        {
            var subscription = messageBus.Subscribe(new BackgroundSemaphoreSubscription<TMessageType>(handlerFunc, concurrencyLevel).HandleMessageAsync);
            return new SubscriptionWrapper<TMessageType>(subscription);
        }

        public static SubscriptionWrapper<TMessageType> CreateBackgroundSemaphoreSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription, int concurrencyLevel = -1)
        {
            var subscription = messageBus.Subscribe(new BackgroundSemaphoreSubscription<TMessageType>(innerSubscription, concurrencyLevel).HandleMessageAsync);
            return new SubscriptionWrapper<TMessageType>(subscription);
        }
    }
}