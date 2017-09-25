namespace Serpent.Common.MessageBus
{
    public class ConcurrentMessageBusOptions<TMessageType>
    {
        public static ConcurrentMessageBusOptions<TMessageType> Default { get; } = new ConcurrentMessageBusOptions<TMessageType>
                                                                                       {
                                                                                           SubscriptionReferenceType =
                                                                                               SubscriptionReferenceTypeType
                                                                                                   .StrongReferences
                                                                                       };

        public BusPublisher<TMessageType> BusPublisher { get; set; } = ParallelPublisher<TMessageType>.Default;

        public SubscriptionReferenceTypeType SubscriptionReferenceType { get; set; } = SubscriptionReferenceTypeType.StrongReferences;

        public WeakReferenceGarbageCollectionOptions WeakReferenceGarbageCollection { get; set; } = WeakReferenceGarbageCollectionOptions.Default;
    }
}