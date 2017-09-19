namespace Serpent.Common.MessageBus
{
    using System;

    public class ConcurrentMessageBusOptions<TMessageType>
    {
        public static ConcurrentMessageBusOptions<TMessageType> Default { get; } = new ConcurrentMessageBusOptions<TMessageType>
        {
                                                                             SubscriptionReferenceType = SubscriptionReferenceTypeType.StrongReferences
                                                                         };

        public BusPublisher<TMessageType> BusPublisher { get; set; } = ParallelPublisher<TMessageType>.Default;

        public SubscriptionReferenceTypeType SubscriptionReferenceType { get; set; }

        public WeakReferenceGarbageCollectionOptions WeakReferenceGarbageCollection { get; set; } = WeakReferenceGarbageCollectionOptions.Default;

        public struct WeakReferenceGarbageCollectionOptions
        {
            public static WeakReferenceGarbageCollectionOptions Default { get; } = new WeakReferenceGarbageCollectionOptions
                                                                                       {
                                                                                           CollectionInterval = TimeSpan.FromSeconds(30),
                                                                                           IsEnabled = true
                                                                                       };

            public TimeSpan CollectionInterval { get; set; }

            public bool IsEnabled { get; set; }
        }
    }
}