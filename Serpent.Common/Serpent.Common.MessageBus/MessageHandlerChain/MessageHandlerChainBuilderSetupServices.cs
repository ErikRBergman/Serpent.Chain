namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using Serpent.Common.MessageBus.Interfaces;

    public struct MessageHandlerChainBuilderSetupServices
    {
        public MessageHandlerChainBuilderSetupServices(IMessageHandlerChainSubscriptionNotification subscriptionNotification)
        {
            this.SubscriptionNotification = subscriptionNotification;
        }

        public IMessageHandlerChainSubscriptionNotification SubscriptionNotification { get; }
    }
}
