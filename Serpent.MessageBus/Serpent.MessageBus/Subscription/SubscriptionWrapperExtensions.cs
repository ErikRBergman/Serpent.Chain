// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using Serpent.MessageHandlerChain;

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
        public static MessageHandlerChainWrapper Wrapper(this IMessageHandlerChain subscription)
        {
            return new MessageHandlerChainWrapper(subscription);
        }
    }
}