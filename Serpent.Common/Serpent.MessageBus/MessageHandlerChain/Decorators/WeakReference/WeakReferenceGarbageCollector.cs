namespace Serpent.MessageBus.MessageHandlerChain.Decorators.WeakReference
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a garbage collector for weak references
    /// </summary>
    public class WeakReferenceGarbageCollector : IWeakReferenceGarbageCollector
    {
        private readonly TimeSpan collectionInterval;

        private readonly CancellationTokenSource shutdownToken = new CancellationTokenSource();

        private readonly ConcurrentDictionary<IWeakReferenceGarbageCollection, bool> weakReferences = new ConcurrentDictionary<IWeakReferenceGarbageCollection, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReferenceGarbageCollector"/> class. 
        /// Uses 60 seconds as the default garbage collection interval
        /// </summary>
        public WeakReferenceGarbageCollector()
            : this(TimeSpan.FromSeconds(60))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReferenceGarbageCollector"/> class. 
        /// </summary>
        /// <param name="collectionInterval">
        /// The garbage collection interval.
        /// </param>
        public WeakReferenceGarbageCollector(TimeSpan collectionInterval)
        {
            this.collectionInterval = collectionInterval;
            this.Start();
        }

        /// <summary>
        /// The default weak references garbage collector, used in situations where a garbage collector is not specified
        /// </summary>
        public static WeakReferenceGarbageCollector Default { get; } = new WeakReferenceGarbageCollector(TimeSpan.FromMinutes(1));

        /// <summary>
        /// Adds a reference to check for garbage collection
        /// </summary>
        /// <param name="weakReference">The weak reference</param>
        public void Add(IWeakReferenceGarbageCollection weakReference)
        {
            this.weakReferences.TryAdd(weakReference, true);
        }

        private void DoGarbageCollection()
        {
            foreach (var weakReference in this.weakReferences.Keys)
            {
                if (weakReference.DisposeSubscriptionIfReclamiedByGarbageCollection())
                {
                    this.weakReferences.TryRemove(weakReference, out var _);
                }
            }
        }

        private async Task GarbageCollectionWorker()
        {
            while (this.shutdownToken.IsCancellationRequested == false)
            {
                await Task.Delay(this.collectionInterval);
                this.DoGarbageCollection();
            }
        }

        private void Start()
        {
            Task.Run(this.GarbageCollectionWorker);
        }
    }
}