namespace Serpent.Common.MessageBus
{
    public class ConcurrentMessageBusOptions<TMessageType>
    {
        public static ConcurrentMessageBusOptions<TMessageType> Default { get; } = new ConcurrentMessageBusOptions<TMessageType>();

        public BusPublisher<TMessageType> BusPublisher { get; set; } = ParallelPublisher<TMessageType>.Default;
    }
}