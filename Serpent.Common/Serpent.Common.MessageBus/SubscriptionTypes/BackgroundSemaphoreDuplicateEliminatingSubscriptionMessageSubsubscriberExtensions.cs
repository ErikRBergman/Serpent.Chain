namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class BackgroundSemaphoreDuplicateEliminatingSubscriptionMessageSubsubscriberExtensions
    {
        public static IMessageBusSubscription CreateBackgroundSemaphoreWithDuplicateEliminationSubscription<TMessageType, TKeyType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector, int concurrencyLevel = -1)
        {
            return messageBus.Subscribe(new BackgroundSemaphoreDuplicateEliminatingSubscription<TMessageType, TKeyType>(handlerFunc, keySelector, concurrencyLevel).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateBackgroundSemaphoreWithDuplicateEliminationSubscription<TMessageType, TKeyType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription, Func<TMessageType, TKeyType> keySelector, int concurrencyLevel = -1)
        {
            return messageBus.Subscribe(new BackgroundSemaphoreDuplicateEliminatingSubscription<TMessageType, TKeyType>(innerSubscription, keySelector, concurrencyLevel).HandleMessageAsync);
        }
    }
}