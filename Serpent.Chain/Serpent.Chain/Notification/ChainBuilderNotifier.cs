namespace Serpent.Chain.Notification
{
    using System;
    using System.Collections.Generic;

    using Serpent.Chain.Interfaces;

    /// <summary>
    /// Provides a notification mechanism for setting up the message handler chain
    /// </summary>
    public class ChainBuilderNotifier : IChainBuilderNotifier
    {
        private readonly List<Action<IChain>> eventSubscribers = new List<Action<IChain>>();

        /// <summary>
        /// Adds a notification subscriber
        /// </summary>
        /// <param name="chain">The method to call when the chain has been created</param>
        public void AddNotification(Action<IChain> chain)
        {
            this.eventSubscribers.Add(chain);
        }

        /// <summary>
        /// Notifies all subscribers of the new chain
        /// </summary>
        /// <param name="chain">The new message handler Chain</param>
        public void Notify(IChain chain)
        {
            foreach (var s in this.eventSubscribers)
            {
                s(chain);
            }
        }
    }
}