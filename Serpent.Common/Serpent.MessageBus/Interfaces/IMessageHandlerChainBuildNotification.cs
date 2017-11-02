namespace Serpent.MessageBus.Interfaces
{
    using System;

    /// <summary>
    /// The message handler chain subscription notification
    /// </summary>
    public interface IMessageHandlerChainBuildNotification
    {
        /// <summary>
        /// Adds an action to call when a message handler chain is created
        /// </summary>
        /// <param name="messageHandlerChain">The newly handled message handler chain</param>
        void AddNotification(Action<IMessageHandlerChain> messageHandlerChain);
    }
}