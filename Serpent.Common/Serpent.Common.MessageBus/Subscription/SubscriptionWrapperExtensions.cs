// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    /// <summary>
    /// Provides extensions for subscription wrapper.
    /// </summary>
    public static class SubscriptionWrapperExtensions 
    {
        /// <summary>
        /// Creates a new subscription wrapper for this subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        /// <returns>A new subscription wrapper</returns>
        public static SubscriptionWrapper Wrapper(this IMessageBusSubscription subscription)
        {
            return new SubscriptionWrapper(subscription);
        }
    }
}