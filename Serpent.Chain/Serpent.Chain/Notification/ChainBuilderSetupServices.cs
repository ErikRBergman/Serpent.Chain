namespace Serpent.Chain.Notification
{
    using Serpent.Chain.Interfaces;

    /// <summary>
    /// The message handler chain builder setup services
    /// </summary>
    public struct ChainBuilderSetupServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChainBuilderSetupServices"/> struct. 
        /// </summary>
        /// <param name="builderNotifier">
        /// The subscription notification service
        /// </param>
        public ChainBuilderSetupServices(IChainBuilderNotifier builderNotifier)
        {
            this.BuilderNotifier = builderNotifier;
        }

        /// <summary>
        /// The subscription notification service
        /// </summary>
        public IChainBuilderNotifier BuilderNotifier { get; }
    }
}
