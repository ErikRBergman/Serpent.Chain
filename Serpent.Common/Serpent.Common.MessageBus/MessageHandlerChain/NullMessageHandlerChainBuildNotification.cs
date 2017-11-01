namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;

    using Serpent.Common.MessageBus.Interfaces;

    internal class NullMessageHandlerChainBuildNotification : IMessageHandlerChainBuildNotification
    {
        public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
        {
        }
    }
}