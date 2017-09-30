// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    public static class SubscriptionWrapperExtensions 
    {
        public static SubscriptionWrapper Wrapper(this IMessageBusSubscription subscription)
        {
            return new SubscriptionWrapper(subscription);
        }
    }
}