namespace Serpent.Chain
{
    using System;

    using Serpent.Chain.Interfaces;

    internal class NullChainBuilderNotifier : IChainBuilderNotifier
    {
        public static IChainBuilderNotifier Default { get; } = new NullChainBuilderNotifier();

        public void AddNotification(Action<IChain> chain)
        {
        }
    }
}