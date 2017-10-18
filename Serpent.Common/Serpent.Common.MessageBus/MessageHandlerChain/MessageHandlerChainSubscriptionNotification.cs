namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;

    using Serpent.Common.MessageBus.Interfaces;

    internal class MessageHandlerChainSubscriptionNotification : IMessageHandlerChainSubscriptionNotification
    {
        private readonly List<Action<IMessageBusSubscription>> subscriptions = new List<Action<IMessageBusSubscription>>();

        public void AddNotification(Action<IMessageBusSubscription> subscription)
        {
            this.subscriptions.Add(subscription);
        }

        public void Notify(IMessageBusSubscription subscription)
        {
            foreach (var s in this.subscriptions)
            {
                s(subscription);
            }
        }
    }
}