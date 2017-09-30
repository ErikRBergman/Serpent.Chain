namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    public static class SubscriptionWrapperExtensions 
    {
        public static SubscriptionWrapper Wrapper(this IMessageBusSubscription subscription)
        {
            return new SubscriptionWrapper(subscription);
        }
    }
}
