namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using Serpent.Common.MessageBus.Interfaces;

    /// <summary>
    /// The message handler chain builder setup services
    /// </summary>
    public struct MessageHandlerChainBuilderSetupServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerChainBuilderSetupServices"/> struct. 
        /// </summary>
        /// <param name="subscriptionNotification">
        /// The subscription notification service
        /// </param>
        public MessageHandlerChainBuilderSetupServices(IMessageHandlerChainSubscriptionNotification subscriptionNotification)
        {
            this.SubscriptionNotification = subscriptionNotification;
        }

        /// <summary>
        /// The subscription notification service
        /// </summary>
        public IMessageHandlerChainSubscriptionNotification SubscriptionNotification { get; }
    }
}
