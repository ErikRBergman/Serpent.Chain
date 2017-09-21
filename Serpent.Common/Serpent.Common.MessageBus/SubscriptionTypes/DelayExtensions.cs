// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class DelayExtensions
    {
        public static SubscriptionBuilder<TMessageType> Delay<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, TimeSpan timeToWait)
        {
            return subscriptionBuilder.Add(currentHandler => new DelaySubscription<TMessageType>(currentHandler, timeToWait).HandleMessageAsync);
        }

        public static SubscriptionBuilder<TMessageType> Delay<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, int timeInMilliseconds)
        {
            return subscriptionBuilder.Add(currentHandler => new DelaySubscription<TMessageType>(currentHandler, TimeSpan.FromMilliseconds(timeInMilliseconds)).HandleMessageAsync);
        }
    }
}