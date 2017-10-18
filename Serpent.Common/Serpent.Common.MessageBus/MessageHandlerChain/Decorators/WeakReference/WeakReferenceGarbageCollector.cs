namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WeakReference
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class WeakReferenceGarbageCollector : IWeakReferenceGarbageCollector
    {
        private readonly TimeSpan collectionInterval;

        private readonly CancellationTokenSource shutdownToken = new CancellationTokenSource();

        private readonly ConcurrentDictionary<IWeakReferenceGarbageCollection, bool> weakReferences = new ConcurrentDictionary<IWeakReferenceGarbageCollection, bool>();

        public WeakReferenceGarbageCollector()
            : this(TimeSpan.FromSeconds(60))
        {
        }

        public WeakReferenceGarbageCollector(TimeSpan collectionInterval)
        {
            this.collectionInterval = collectionInterval;
            this.Start();
        }

        public static WeakReferenceGarbageCollector Default { get; } = new WeakReferenceGarbageCollector(TimeSpan.FromMinutes(1));

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