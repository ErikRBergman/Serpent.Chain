namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    public static class SubscriptionWrapperExtensions 
    {
        public static SubscriptionWrapper Wrapper(this IMessageBusSubscription subscription)
        {
            return new SubscriptionWrapper(subscription);
        }
    }
}
