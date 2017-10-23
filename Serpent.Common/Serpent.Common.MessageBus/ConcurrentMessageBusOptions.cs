namespace Serpent.Common.MessageBus
{
    /// <summary>
    /// The concurrent message bus options
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class ConcurrentMessageBusOptions<TMessageType>
    {
        /// <summary>
        /// The publisher, distributing messages to the subscribers
        /// </summary>
        public BusPublisher<TMessageType> BusPublisher { get; set; } = ParallelPublisher<TMessageType>.Default;

        internal static ConcurrentMessageBusOptions<TMessageType> Default { get; } = new ConcurrentMessageBusOptions<TMessageType>();
    }
}