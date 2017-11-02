namespace Serpent.MessageBus.MessageHandlerChain
{
    using Serpent.MessageBus.Interfaces;

    /// <summary>
    /// The message handler chain builder setup services
    /// </summary>
    public struct MessageHandlerChainBuilderSetupServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerChainBuilderSetupServices"/> struct. 
        /// </summary>
        /// <param name="buildNotification">
        /// The subscription notification service
        /// </param>
        public MessageHandlerChainBuilderSetupServices(IMessageHandlerChainBuildNotification buildNotification)
        {
            this.BuildNotification = buildNotification;
        }

        /// <summary>
        /// The subscription notification service
        /// </summary>
        public IMessageHandlerChainBuildNotification BuildNotification { get; }
    }
}
