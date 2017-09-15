 // ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class BackgroundSemaphoreSubscriptionIMessageBusSubscriberExtensions
    {
        public static SubscriptionWrapper<T> CreateBackgroundSemaphoreSubscription<T>(this IMessageBusSubscriber<T> messageBus, Func<T, Task> handlerFunc, int concurrencyLevel = -1)
        {
            var subscription = messageBus.Subscribe(new BackgroundSemaphoreSubscription<T>(handlerFunc, concurrencyLevel).HandleMessageAsync);
            return new SubscriptionWrapper<T>(subscription);
        }

        public static SubscriptionWrapper<T> CreateBackgroundSemaphoreSubscription<T>(this IMessageBusSubscriber<T> messageBus, BusSubscription<T> innerSubscription, int concurrencyLevel = -1)
        {
            var subscription = messageBus.Subscribe(new BackgroundSemaphoreSubscription<T>(innerSubscription, concurrencyLevel).HandleMessageAsync);
            return new SubscriptionWrapper<T>(subscription);
        }
    }
}