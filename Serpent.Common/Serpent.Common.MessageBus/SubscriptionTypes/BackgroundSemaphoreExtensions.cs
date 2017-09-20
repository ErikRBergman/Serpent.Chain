 // ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class BackgroundSemaphoreExtensions
    {
        public static IMessageBusSubscription CreateBackgroundSemaphoreSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc, int concurrencyLevel = -1)
        {
            return messageBus.Subscribe(new BackgroundSemaphoreSubscription<TMessageType>(handlerFunc, concurrencyLevel).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateBackgroundSemaphoreSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription, int concurrencyLevel = -1)
        {
            return messageBus.Subscribe(new BackgroundSemaphoreSubscription<TMessageType>(innerSubscription, concurrencyLevel).HandleMessageAsync);
        }
    }
}