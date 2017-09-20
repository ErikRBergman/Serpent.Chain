// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class LimitedThroughputExtensions
    {
        public static SubscriptionBuilder<TMessageType> LimitedThroughput<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            int maxMessagesPerPeriod,
            TimeSpan? periodSpan = null)
        {
            return subscriptionBuilder.Add(
                currentHandler => new LimitedThroughputSubscription<TMessageType>(currentHandler, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateLimitedThroughputSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc, int maxMessagesPerPeriod, TimeSpan? periodSpan = null)
        {
            return messageBus.Subscribe(new LimitedThroughputSubscription<TMessageType>(handlerFunc, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateLimitedThroughputSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, IMessageHandler<TMessageType> handler, int maxMessagesPerPeriod, TimeSpan? periodSpan = null)
        {
            return messageBus.Subscribe(new LimitedThroughputSubscription<TMessageType>(handler.HandleMessageAsync, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateLimitedThroughputSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription, int maxMessagesPerPeriod, TimeSpan? periodSpan = null)
        {
            return messageBus.Subscribe(new LimitedThroughputSubscription<TMessageType>(innerSubscription, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)).HandleMessageAsync);
        }
    }
}