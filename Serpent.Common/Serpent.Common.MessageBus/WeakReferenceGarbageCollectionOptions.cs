namespace Serpent.Common.MessageBus
{
    using System;

    public struct WeakReferenceGarbageCollectionOptions
    {
        public static WeakReferenceGarbageCollectionOptions Default { get; } = new WeakReferenceGarbageCollectionOptions
        {
            CollectionInterval = TimeSpan.FromSeconds(30),
        };

        public TimeSpan CollectionInterval { get; set; }

        public bool IsEnabled { get; set; }
    }
}