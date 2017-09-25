// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class LimitedThroughputExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> LimitedThroughput<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxMessagesPerPeriod,
            TimeSpan? periodSpan = null)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new LimitedThroughputSubscription<TMessageType>(currentHandler, maxMessagesPerPeriod, periodSpan ?? TimeSpan.FromSeconds(1)).HandleMessageAsync);
        }
    }
}