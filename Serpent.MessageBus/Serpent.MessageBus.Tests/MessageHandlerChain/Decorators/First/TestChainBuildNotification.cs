namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.First
{
    using System;

    using Serpent.MessageBus.Interfaces;

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