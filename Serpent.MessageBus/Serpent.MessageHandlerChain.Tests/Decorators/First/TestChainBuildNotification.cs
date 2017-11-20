namespace Serpent.MessageHandlerChain.Tests.Decorators.First
{
    using System;

    using Serpent.MessageHandlerChain.Interfaces;

    internal class TestChainBuildNotification : IMessageHandlerChainBuildNotification, IMessageHandlerChain
    {
        public bool IsDisposed { get; private set; }

        public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
        {
            messageHandlerChain(this);
        }

        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }
}