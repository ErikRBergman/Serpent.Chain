// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class RetrySubscriptionMessageBusSubscriberExtensions
    {
        public static IMessageBusSubscription CreateRetrySubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc, int maxNumberOfAttempts, TimeSpan retryDelay)
        {
            return messageBus.Subscribe(new RetrySubscription<TMessageType>(handlerFunc, maxNumberOfAttempts, retryDelay).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateRetrySubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, IMessageHandler<TMessageType> handler, int maxNumberOfAttempts, TimeSpan retryDelay)
        {
            return messageBus.Subscribe(new RetrySubscription<TMessageType>(handler.HandleMessageAsync, maxNumberOfAttempts, retryDelay).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateRetrySubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription, int maxNumberOfAttempts, TimeSpan retryDelay)
        {
            return messageBus.Subscribe(new RetrySubscription<TMessageType>(innerSubscription, maxNumberOfAttempts, retryDelay).HandleMessageAsync);
        }
    }
}