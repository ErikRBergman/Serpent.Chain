namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.BusPublishers;

    public class ConcurrentMessageBus<T> : IMessageBus<T>
    {
        private readonly ConcurrentMessageBusOptions concurrentMessageBusOptions = ConcurrentMessageBusOptions.Default;

        private readonly ExclusiveAccess<int> currentSubscriptionId = new ExclusiveAccess<int>(0);

        private readonly Func<IEnumerable<ISubscription<T>>, T, Task> publishAsyncFunc;

        private readonly ConcurrentQueue<int> recycledSubscriptionIds = new ConcurrentQueue<int>();

        private readonly ConcurrentDictionary<int, ISubscription<T>> subscribers = new ConcurrentDictionary<int, ISubscription<T>>();

        public ConcurrentMessageBus(ConcurrentMessageBusOptions concurrentMessageBusOptions)
            : this()
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
                this.publishAsyncFunc = this.GetPublishFunc(this.concurrentMessageBusOptions.PublishType);
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

        public Task PublishAsync(T message) => this.publishAsyncFunc(this.subscribers.Values, message);

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

        private ISubscription<T> CreateSubscription(Func<T, Task> subscriptionHandlerFunc)
        {
            return this.concurrentMessageBusOptions.SubscriptionReferenceType != ConcurrentMessageBusOptions.SubscriptionReferenceTypeType.WeakReferences
                       ? new StrongReferenceSubscription(subscriptionHandlerFunc)
                       : (ISubscription<T>)new WeakReferenceSubscription(subscriptionHandlerFunc);
        }

        private int GetNewSubscriptionId()
        {
            if (!this.recycledSubscriptionIds.TryDequeue(out var result))
            {
                result = this.currentSubscriptionId.Increment();
            }

            return result;
        }

        private Func<IEnumerable<ISubscription<T>>, T, Task> GetPublishFunc(ConcurrentMessageBusOptions.PublishTypeType publishTypeType)
        {
            switch (publishTypeType)
            {
                case ConcurrentMessageBusOptions.PublishTypeType.Serial:
                    return SerialPublisher<T>.Default.PublishAsync;
                case ConcurrentMessageBusOptions.PublishTypeType.Parallel:
                    return ParallelPublisher<T>.Default.PublishAsync;
                case ConcurrentMessageBusOptions.PublishTypeType.ForcedParallel:
                    return ForcedParallelPublisher<T>.Default.PublishAsync;
                case ConcurrentMessageBusOptions.PublishTypeType.Custom:

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

            public void Dispose()
            {
                this.Unsubscribe();
            }
        }

        private struct StrongReferenceSubscription : ISubscription<T>
        {
            public StrongReferenceSubscription(Func<T, Task> subscriptionHandlerFunc)
            {
                this.SubscriptionHandlerFunc = subscriptionHandlerFunc;
            }

            public Func<T, Task> SubscriptionHandlerFunc { get; set; }
        }

        private struct WeakReferenceSubscription : ISubscription<T>
        {
            private readonly WeakReference<Func<T, Task>> subscriptionHandlerFunc;

            public WeakReferenceSubscription(Func<T, Task> subscriptionHandlerFunc)
            {
                this.subscriptionHandlerFunc = new WeakReference<Func<T, Task>>(subscriptionHandlerFunc);
            }

            public Func<T, Task> SubscriptionHandlerFunc
            {
                get
                {
                    if (this.subscriptionHandlerFunc.TryGetTarget(out var target))
                    {
                        return target;
                    }

                    return null;
                }
            }
        }

        public class ConcurrentMessageBusOptions
        {
            public enum PublishTypeType
            {
                /// <summary>
                /// A message is sent to the handler functions serially, 
                /// which means the first subscriber must finish handling the message before the second handler is invoked.
                /// The Task returned by PublishAsync is done when all handlers Tasks are done
                /// </summary>
                Serial,

                /// <summary>
                /// A message is sent to all handler functions serially, not awaiting each handler to finish before the next handler is invoked
                /// This is the DEFAULT value. If all handlers are written to follow the TPL guidelines (never blocking), this is usually the best option. 
                /// The Task returned by PublishAsync is done when all handlers Tasks are done
                /// </summary>
                Parallel,

                /// <summary>
                /// A message is sent to all handler functions in parallel, not awaiting each handler to finish before the next handler is invoked
                /// This is the DEFAULT value. If all handlers are written to follow the TPL guidelines (never blocking), this is usually the best option.
                /// The Task returned by PublishAsync is done when all handlers Tasks are done
                /// </summary>
                ForcedParallel,

                /// <summary>
                /// Use this option to use the CustomBusPublisher property for a custom publisher
                /// </summary>
                Custom
            }

            public enum SubscriptionReferenceTypeType
            {
                StrongReferences,

                WeakReferences
            }

            public static ConcurrentMessageBusOptions Default { get; } = new ConcurrentMessageBusOptions
            {
                PublishType = PublishTypeType.Parallel,
                SubscriptionReferenceType = SubscriptionReferenceTypeType.StrongReferences
            };

            public BusPublisher<T> CustomBusPublisher { get; set; }

            public PublishTypeType PublishType { get; set; } = PublishTypeType.Parallel;

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