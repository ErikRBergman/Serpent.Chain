// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class NoDuplicatesExtensions
    {
        public static SubscriptionBuilder<TMessageType> NoDuplicates<TMessageType, TKeyType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, Func<TMessageType, TKeyType> keySelector)
        {
            return subscriptionBuilder.Add(currentHandler => new NoDuplicatesSubscription<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        public static SubscriptionBuilder<TMessageType> NoDuplicates<TMessageType, TKeyType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            return subscriptionBuilder.Add(currentHandler => new NoDuplicatesSubscription<TMessageType, TKeyType>(currentHandler, keySelector, equalityComparer));
        }
    }
}