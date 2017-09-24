// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class BranchExtensions
    {
        public static SubscriptionBuilder<TMessageType> Branch<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            params Action<SubscriptionBuilder<TMessageType>>[] branches)
        {
            return subscriptionBuilder.Add(currentHandler => new BranchSubscription<TMessageType>(currentHandler, branches));
        }
    }
}