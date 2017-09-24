// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

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
    }
}