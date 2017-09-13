namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.BusPublishers;

    public class ConcurrentMessageBus<T> : IMessageBus<T>
    {
        private readonly ConcurrentQueue<int> recycledSubscriptionIds = new ConcurrentQueue<int>();

        private readonly ConcurrentEventBusOptions concurrentEventBusOptions = ConcurrentEventBusOptions.Default;

        private readonly ExclusiveAccess<int> currentSubscriptionId = new ExclusiveAccess<int>(0);

        private readonly Func<IEnumerable<ISubscription<T>>, T, Task> publishAsyncFunc;

        private readonly ConcurrentDictionary<int, ISubscription<T>> subscribers = new ConcurrentDictionary<int, ISubscription<T>>();

        public ConcurrentMessageBus(ConcurrentEventBusOptions concurrentEventBusOptions) : this()
        {
            this.concurrentEventBusOptions = concurrentEventBusOptions;
        }

        public ConcurrentMessageBus()
        {
            this.publishAsyncFunc = this.GetPublishFunc(this.concurrentEventBusOptions);
        }

        public ConcurrentMessageBus(ConcurrentEventBusOptions concurrentEventBusOptions, Func<IEnumerable<ISubscription<T>>, T, Task> publishFunction)
        {
            this.concurrentEventBusOptions = concurrentEventBusOptions;
            this.publishAsyncFunc = publishFunction;
        }

        public ConcurrentMessageBus(ConcurrentEventBusOptions concurrentEventBusOptions, BusPublisher<T> publisher)
        {
            this.concurrentEventBusOptions = concurrentEventBusOptions;
            this.publishAsyncFunc = publisher.PublishAsync;
        }

        public int SubscriberCount => this.subscribers.Count;

        public Task PublishEventAsync(T eventData) => this.publishAsyncFunc(this.subscribers.Values, eventData);

        public IMessageBusSubscription Subscribe(Func<T, Task> invocationFunc)
        {
            var subscription = this.CreateSubscription(invocationFunc);

            var newSubscriptionId = this.GetNewSubscriptionId();

            this.subscribers.TryAdd(newSubscriptionId, subscription);

            return this.CreateMessageBusSubscription(newSubscriptionId);
        }

        private ConcurrentMessageBusSubscription CreateMessageBusSubscription(int newSubscriptionId)
        {
            return new ConcurrentMessageBusSubscription(() => this.Unsubscribe(newSubscriptionId));
        }

        private ISubscription<T> CreateSubscription(Func<T, Task> eventSubscription)
        {
            return this.concurrentEventBusOptions.SubscriptionReferenceType != ConcurrentEventBusOptions.SubscriptionReferenceTypeType.WeakReferences
                       ? new StrongReferenceSubscription(eventSubscription)
                       : (ISubscription<T>)new WeakReferenceSubscription(eventSubscription);
        }

        private int GetNewSubscriptionId()
        {
            if (!this.recycledSubscriptionIds.TryDequeue(out var result))
            {
                result = this.currentSubscriptionId.Increment();
            }

            return result;
        }

        private Func<IEnumerable<ISubscription<T>>, T, Task> GetPublishFunc(ConcurrentEventBusOptions eventBusOptions)
        {
            switch (eventBusOptions.InvokationMethod)
            {
                case ConcurrentEventBusOptions.InvokationMethodType.Serial:
                    return SerialPublisher<T>.Default.PublishAsync;
                case ConcurrentEventBusOptions.InvokationMethodType.Parallel:
                    return ParallelPublisher<T>.Default.PublishAsync;
                case ConcurrentEventBusOptions.InvokationMethodType.ForcedParallel:
                    return ForcedParallelPublisher<T>.Default.PublishAsync;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Unsubscribe(int subscriptionId)
        {
            this.currentSubscriptionId.Update(
                v =>
                    {
                        if (this.subscribers.TryRemove(subscriptionId, out _))
                        {
                            if (subscriptionId == v)
                            {
                                --v;
                            }
                            else
                            {
                                this.recycledSubscriptionIds.Enqueue(subscriptionId);
                            }
                        }

                        return v;
                    });
        }

        private struct ConcurrentMessageBusSubscription : IMessageBusSubscription
        {
            private static readonly Action DoNothing = () => { };

            private Action unsubscribeAction;

            public ConcurrentMessageBusSubscription(Action unsubscribeAction)
            {
                this.unsubscribeAction = unsubscribeAction;
            }

            public void Unsubscribe()
            {
                this.unsubscribeAction.Invoke();
                this.unsubscribeAction = DoNothing;
            }
        }

        private struct StrongReferenceSubscription : ISubscription<T>
        {
            public StrongReferenceSubscription(Func<T, Task> eventInvocationFunc)
            {
                this.EventInvocationFunc = eventInvocationFunc;
            }

            public Func<T, Task> EventInvocationFunc { get; set; }
        }

        private struct WeakReferenceSubscription : ISubscription<T>
        {
            private readonly WeakReference<Func<T, Task>> eventInvocationFunc;

            public WeakReferenceSubscription(Func<T, Task> eventInvocationFunc)
            {
                this.eventInvocationFunc = new WeakReference<Func<T, Task>>(eventInvocationFunc);
            }

            public Func<T, Task> EventInvocationFunc
            {
                get
                {
                    if (this.eventInvocationFunc.TryGetTarget(out var target))
                    {
                        return target;
                    }

                    return null;
                }
            }
        }

        public class ConcurrentEventBusOptions
        {
            public enum InvokationMethodType
            {
                Serial,

                Parallel,

                ForcedParallel
            }

            public enum SubscriptionReferenceTypeType
            {
                StrongReferences,

                WeakReferences
            }

            public static ConcurrentEventBusOptions Default { get; } = new ConcurrentEventBusOptions
                                                                           {
                                                                               InvokationMethod = InvokationMethodType.Parallel,
                                                                               SubscriptionReferenceType =
                                                                                   SubscriptionReferenceTypeType.StrongReferences,
                                                                           };

            public InvokationMethodType InvokationMethod { get; set; } = InvokationMethodType.Parallel;

            public SubscriptionReferenceTypeType SubscriptionReferenceType { get; set; }

            public WeakReferenceGarbageCollectionOptions WeakReferenceGarbageCollection { get; set; } = WeakReferenceGarbageCollectionOptions.Default;

            public struct WeakReferenceGarbageCollectionOptions
            {
                public static WeakReferenceGarbageCollectionOptions Default { get; } = new WeakReferenceGarbageCollectionOptions
                                                                                           {
                                                                                               CollectionInterval =
                                                                                                   TimeSpan.FromSeconds(30),
                                                                                               IsEnabled = true
                                                                                           };

                public TimeSpan CollectionInterval { get; set; }

                public bool IsEnabled { get; set; }
            }
        }
    }
}