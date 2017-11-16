namespace Serpent.MessageBus.MessageHandlerChain
{
    using System;

    using Serpent.MessageBus.Interfaces;

    internal class NullMessageHandlerChainBuildNotification : IMessageHandlerChainBuildNotification
    {
        public static IMessageHandlerChainBuildNotification Default { get; } = new NullMessageHandlerChainBuildNotification();

        public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
        {
        }
    }
}