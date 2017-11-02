namespace Serpent.MessageBus.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;

    using Serpent.MessageBus.Interfaces;

    internal class MessageHandlerChainBuildNotification : IMessageHandlerChainBuildNotification
    {
        private readonly List<Action<IMessageHandlerChain>> eventSubscribers = new List<Action<IMessageHandlerChain>>();

        public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
        {
            this.eventSubscribers.Add(messageHandlerChain);
        }

        public void Notify(IMessageHandlerChain messageHandlerChain)
        {
            foreach (var s in this.eventSubscribers)
            {
                s(messageHandlerChain);
            }
        }
    }
}