namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.Helpers;
    using Serpent.Common.MessageBus.Models;

    public class ConcurrentMessageBusOptions<TMessageType>
    {
        public static ConcurrentMessageBusOptions<TMessageType> Default { get; } = new ConcurrentMessageBusOptions<TMessageType>
        {
            SubscriptionReferenceType = SubscriptionReferenceTypeType.StrongReferences
        };

        public BusPublisher<TMessageType> BusPublisher { get; set; } = ParallelPublisher<TMessageType>.Default;

        public SubscriptionReferenceTypeType SubscriptionReferenceType { get; set; } = SubscriptionReferenceTypeType.StrongReferences;

        public WeakReferenceGarbageCollectionOptions WeakReferenceGarbageCollection { get; set; } = WeakReferenceGarbageCollectionOptions.Default;

    }
}