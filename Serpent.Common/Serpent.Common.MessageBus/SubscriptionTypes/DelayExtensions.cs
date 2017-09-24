// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class DelayExtensions
    {
        public static SubscriptionBuilder<TMessageType> Delay<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, TimeSpan timeToWait, bool dontAwait = false)
        {
            return subscriptionBuilder.Add(currentHandler => new DelaySubscription<TMessageType>(currentHandler, timeToWait, dontAwait).HandleMessageAsync);
        }

        public static SubscriptionBuilder<TMessageType> Delay<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            int timeInMilliseconds,
            bool dontAwait = false)
        {
            return subscriptionBuilder.Add(
                currentHandler => new DelaySubscription<TMessageType>(currentHandler, TimeSpan.FromMilliseconds(timeInMilliseconds), dontAwait).HandleMessageAsync);
        }
    }
}