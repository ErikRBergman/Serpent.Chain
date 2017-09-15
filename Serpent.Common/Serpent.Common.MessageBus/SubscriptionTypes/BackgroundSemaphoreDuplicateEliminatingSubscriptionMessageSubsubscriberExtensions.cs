namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class BackgroundSemaphoreDuplicateEliminatingSubscriptionMessageSubsubscriberExtensions
    {
        public static SubscriptionWrapper<TMessageType> CreateBackgroundSemaphoreWithDuplicateEliminationSubscription<TMessageType, TKeyType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector, int concurrencyLevel = -1)
        {
            var subscription = messageBus.Subscribe(new BackgroundSemaphoreDuplicateEliminatingSubscription<TMessageType, TKeyType>(handlerFunc, keySelector, concurrencyLevel).HandleMessageAsync);
            return new SubscriptionWrapper<TMessageType>(subscription);
        }

        public static SubscriptionWrapper<TMessageType> CreateBackgroundSemaphoreWithDuplicateEliminationSubscription<TMessageType, TKeyType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription, Func<TMessageType, TKeyType> keySelector, int concurrencyLevel = -1)
        {
            var subscription = messageBus.Subscribe(new BackgroundSemaphoreDuplicateEliminatingSubscription<TMessageType, TKeyType>(innerSubscription, keySelector, concurrencyLevel).HandleMessageAsync);
            return new SubscriptionWrapper<TMessageType>(subscription);
        }
    }
}