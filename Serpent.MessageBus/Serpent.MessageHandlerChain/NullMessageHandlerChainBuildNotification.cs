namespace Serpent.MessageHandlerChain
{
    using System;

    using Serpent.MessageHandlerChain.Interfaces;

    internal class NullMessageHandlerChainBuildNotification : IMessageHandlerChainBuildNotification
    {
        public static IMessageHandlerChainBuildNotification Default { get; } = new NullMessageHandlerChainBuildNotification();

        public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
        {
        }
    }
}