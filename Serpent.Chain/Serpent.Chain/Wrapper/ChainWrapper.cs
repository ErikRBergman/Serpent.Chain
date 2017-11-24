//// ReSharper disable CheckNamespace

namespace Serpent.Chain
{
    /// <summary>
    ///     The subscription wrapper type. Unsubscribes when disposed or runs out of scope
    /// </summary>
    public struct ChainWrapper : IChain
    {
        private IChain chain;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChainWrapper" /> class.
        /// </summary>
        /// <param name="chain">
        ///     The subscription
        /// </param>
        public ChainWrapper(IChain chain)
        {
            this.chain = chain;
        }

        /// <summary>
        ///     Creates a new subscription wrapper
        /// </summary>
        /// <param name="chain">The message handler Chain</param>
        /// <returns>A subscription wrapper</returns>
        public static ChainWrapper Create(IChain chain)
        {
            return new ChainWrapper(chain);
        }

        /// <summary>
        ///     Disposes the object
        /// </summary>
        public void Dispose()
        {
            this.Unsubscribe();
        }

        /// <summary>
        ///     Unsubscribes to the message bus
        /// </summary>
        public void Unsubscribe()
        {
            this.chain?.Dispose();
            this.chain = null;
        }
    }
}