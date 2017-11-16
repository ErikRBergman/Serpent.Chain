namespace Serpent.MessageBus.Helpers
{
    /// <summary>
    ///  Provides a subscription that does nothing
    /// </summary>
    public class NullMessageBusSubscription : IMessageBusSubscription
    {
        private NullMessageBusSubscription()
        {
        }

        /// <summary>
        ///  The null message bus subscription singleton
        /// </summary>
        public static IMessageBusSubscription Default { get; } = new NullMessageBusSubscription();

        /// <summary>
        /// Does nothing
        /// </summary>
        public void Dispose()
        {
        }
    }
}