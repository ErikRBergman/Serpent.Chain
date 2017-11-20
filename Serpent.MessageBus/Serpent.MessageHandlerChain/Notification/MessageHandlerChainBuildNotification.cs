namespace Serpent.MessageHandlerChain.Notification
{
    using System;
    using System.Collections.Generic;

    using Serpent.MessageHandlerChain.Interfaces;

    /// <summary>
    /// Provides a notification mechanism for setting up the message handler chain
    /// </summary>
    public class MessageHandlerChainBuildNotification : IMessageHandlerChainBuildNotification
    {
        private readonly List<Action<IMessageHandlerChain>> eventSubscribers = new List<Action<IMessageHandlerChain>>();

        /// <summary>
        /// Adds a notification subscriber
        /// </summary>
        /// <param name="messageHandlerChain">The method to call when the chain has been created</param>
        public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
        {
            this.eventSubscribers.Add(messageHandlerChain);
        }

        /// <summary>
        /// Notifies all subscribers of the new chain
        /// </summary>
        /// <param name="messageHandlerChain">The new message handler chain</param>
        public void Notify(IMessageHandlerChain messageHandlerChain)
        {
            foreach (var s in this.eventSubscribers)
            {
                s(messageHandlerChain);
            }
        }
    }
}