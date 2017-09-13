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

        private readonly ConcurrentMessageBusOptions concurrentMessageBusOptions = ConcurrentMessageBusOptions.Default;

        private readonly ExclusiveAccess<int> currentSubscriptionId = new ExclusiveAccess<int>(0);

        private readonly Func<IEnumerable<ISubscription<T>>, T, Task> publishAsyncFunc;

        private readonly ConcurrentDictionary<int, ISubscription<T>> subscribers = new ConcurrentDictionary<int, ISubscription<T>>();

        public ConcurrentMessageBus(ConcurrentMessageBusOptions concurrentMessageBusOptions) : this()
        {
            this.concurrentMessageBusOptions = concurrentMessageBusOptions;
        }

        public ConcurrentMessageBus()
        {
            if (this.concurrentMessageBusOptions.CustomBusPublisher != null)
            {
                this.publishAsyncFunc = this.concurrentMessageBusOptions.CustomBusPublisher.PublishAsync;
            }
            else
            {
                this.publishAsyncFunc = this.GetPublishFunc(this.concurrentMessageBusOptions.InvokationMethod);
            }
        }

        public ConcurrentMessageBus(ConcurrentMessageBusOptions concurrentMessageBusOptions, Func<IEnumerable<ISubscription<T>>, T, Task> publishFunction)
        {
            this.concurrentMessageBusOptions = concurrentMessageBusOptions;
            this.publishAsyncFunc = publishFunction;
        }

        public ConcurrentMessageBus(ConcurrentMessageBusOptions concurrentMessageBusOptions, BusPublisher<T> publisher)
        {
            this.concurrentMessageBusOptions = concurrentMessageBusOptions;
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
            return this.concurrentMessageBusOptions.SubscriptionReferenceType != ConcurrentMessageBusOptions.SubscriptionReferenceTypeType.WeakReferences
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

        private Func<IEnumerable<ISubscription<T>>, T, Task> GetPublishFunc(ConcurrentMessageBusOptions.InvokationMethodType invokationMethodType)
        {
            switch (invokationMethodType)
            {
                case ConcurrentMessageBusOptions.InvokationMethodType.Serial:
                    return SerialPublisher<T>.Default.PublishAsync;
                case ConcurrentMessageBusOptions.InvokationMethodType.Parallel:
                    return ParallelPublisher<T>.Default.PublishAsync;
                case ConcurrentMessageBusOptions.InvokationMethodType.ForcedParallel:
                    return ForcedParallelPublisher<T>.Default.PublishAsync;
                case ConcurrentMessageBusOptions.InvokationMethodType.Custom:

                    if (this.concurrentMessageBusOptions.CustomBusPublisher == null)
                    {
                        throw new Exception("The custom bus publisher must be set to use a custom bus publisher");
                    }

                    return this.concurrentMessageBusOptions.CustomBusPublisher.PublishAsync;
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

        public class ConcurrentMessageBusOptions
        {
            public enum InvokationMethodType
            {
                Serial,

                Parallel,

                ForcedParallel,

                Custom
            }

            public enum SubscriptionReferenceTypeType
            {
                StrongReferences,

                WeakReferences
            }

            public BusPublisher<T> CustomBusPublisher { get; set; }

            public static ConcurrentMessageBusOptions Default { get; } = new ConcurrentMessageBusOptions
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