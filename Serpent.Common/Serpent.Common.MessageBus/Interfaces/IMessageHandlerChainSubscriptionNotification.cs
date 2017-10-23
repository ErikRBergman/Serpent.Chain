namespace Serpent.Common.MessageBus.Interfaces
{
    using System;

    /// <summary>
    /// The message handler chain subscription notification
    /// </summary>
    public interface IMessageHandlerChainSubscriptionNotification
    {
        /// <summary>
        /// Adds an action to call when a subscription is created
        /// </summary>
        /// <param name="subscription">The subscription</param>
        void AddNotification(Action<IMessageBusSubscription> subscription);
    }
}