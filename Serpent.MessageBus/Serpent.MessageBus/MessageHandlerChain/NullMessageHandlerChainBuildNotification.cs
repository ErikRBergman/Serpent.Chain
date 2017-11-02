namespace Serpent.MessageBus.MessageHandlerChain
{
    using System;

    using Serpent.MessageBus.Interfaces;

    internal class NullMessageHandlerChainBuildNotification : IMessageHandlerChainBuildNotification
    {
        public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
        {
        }
    }
}