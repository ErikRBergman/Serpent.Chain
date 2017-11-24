namespace Serpent.Chain.Interfaces
{
    using System;

    /// <summary>
    /// The message handler chain subscription notification
    /// </summary>
    public interface IChainBuilderNotifier
    {
        /// <summary>
        /// Adds an action to call when a message handler chain is created
        /// </summary>
        /// <param name="chain">The newly handled message handler Chain</param>
        void AddNotification(Action<IChain> chain);
    }
}