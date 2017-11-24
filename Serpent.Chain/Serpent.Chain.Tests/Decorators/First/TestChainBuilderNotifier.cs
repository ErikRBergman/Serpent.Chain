namespace Serpent.Chain.Tests.Decorators.First
{
    using System;

    using Serpent.Chain.Interfaces;

    internal class TestChainBuilderNotifier : IChainBuilderNotifier, IChain
    {
        public bool IsDisposed { get; private set; }

        public void AddNotification(Action<IChain> chain)
        {
            if (chain == null)
            {
                throw new ArgumentNullException(nameof(chain));
            }

            chain(this);
        }

        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }
}