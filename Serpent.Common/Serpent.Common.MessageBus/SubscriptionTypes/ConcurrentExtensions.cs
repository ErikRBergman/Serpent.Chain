 // ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class ConcurrentExtensions
    {
        public static SubscriptionBuilder<TMessageType> Concurrent<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, int maxNumberOfConcurrentMessages)
        {
            return subscriptionBuilder.Add(currentHandler => new ConcurrentSubscription<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }

        public static IMessageBusSubscription CreateConcurrentSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc, int concurrencyLevel = -1)
        {
            return messageBus.Subscribe(new ConcurrentSubscription<TMessageType>(handlerFunc, concurrencyLevel).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateConcurrentSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription, int concurrencyLevel = -1)
        {
            return messageBus.Subscribe(new ConcurrentSubscription<TMessageType>(innerSubscription, concurrencyLevel).HandleMessageAsync);
        }
    }
}