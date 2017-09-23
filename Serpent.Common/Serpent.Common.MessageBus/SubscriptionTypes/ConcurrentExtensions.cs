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
    }
}