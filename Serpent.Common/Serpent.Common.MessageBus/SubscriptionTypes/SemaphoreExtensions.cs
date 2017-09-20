// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class SemapshoreExtensions
    {
        public static SubscriptionBuilder<TMessageType> Semaphore<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, int maxNumberOfConcurrentMessages)
        {
            return subscriptionBuilder.Add(currentHandler => new SemaphoreSubscription<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }
    }
}