namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Helpers;

    public class ConcurrentMessageBus<TMessageType> : IMessageBus<TMessageType>
    {
        private readonly ConcurrentMessageBusOptions<TMessageType> options = ConcurrentMessageBusOptions<TMessageType>.Default;

        private readonly ExclusiveAccess<int> currentSubscriptionId = new ExclusiveAccess<int>(0);

        private readonly Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, Task> publishAsyncFunc;

        private readonly ConcurrentQueue<int> recycledSubscriptionIds = new ConcurrentQueue<int>();

        private readonly ConcurrentDictionary<int, ISubscription<TMessageType>> subscriptions = new ConcurrentDictionary<int, ISubscription<TMessageType>>();

        private readonly object lockObject = new object();

        private IEnumerable<ISubscription<TMessageType>> subscriptionCache = Array.Empty<ISubscription<TMessageType>>();

        public ConcurrentMessageBus(ConcurrentMessageBusOptions<TMessageType> options)
        {
            this.options = options;
            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        public ConcurrentMessageBus(Action<ConcurrentMessageBusOptions<TMessageType>> optionsFunc)
        {
            var newOptions = new ConcurrentMessageBusOptions<TMessageType>();
            optionsFunc(newOptions);
            this.options = newOptions;
            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        public ConcurrentMessageBus()
        {
            if (this.options.BusPublisher == null)
            {
                throw new Exception("No BusPublisher must not be null");
            }

            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        public ConcurrentMessageBus(ConcurrentMessageBusOptions<TMessageType> options, Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, Task> publishFunction)
        {
            this.options = options;
            this.publishAsyncFunc = publishFunction;
        }

        public ConcurrentMessageBus(ConcurrentMessageBusOptions<TMessageType> options, BusPublisher<TMessageType> publisher)
        {
            this.options = options;
            this.publishAsyncFunc = publisher.PublishAsync;
        }

        public int SubscriberCount => this.subscriptions.Count;

        public Task PublishAsync(TMessageType message) => this.publishAsyncFunc(this.subscriptionCache, message);

        public IMessageBusSubscription Subscribe(Func<TMessageType, Task> invocationFunc)
        {
            var subscription = this.CreateSubscription(invocationFunc);

            var newSubscriptionId = this.GetNewSubscriptionId();

            this.subscriptions.TryAdd(newSubscriptionId, subscription);

            lock (this.lockObject)
            {
                this.subscriptionCache = this.subscriptions.Values;
            }

            return this.CreateMessageBusSubscription(newSubscriptionId);
        }

        private ConcurrentMessageBusSubscription CreateMessageBusSubscription(int newSubscriptionId)
        {
            return new ConcurrentMessageBusSubscription(() => this.Unsubscribe(newSubscriptionId));
        }

        private ISubscription<TMessageType> CreateSubscription(Func<TMessageType, Task> subscriptionHandlerFunc)
        {
            return this.options.SubscriptionReferenceType != SubscriptionReferenceTypeType.WeakReferences
                       ? new StrongReferenceSubscription(subscriptionHandlerFunc)
                       : (ISubscription<TMessageType>)new WeakReferenceSubscription(subscriptionHandlerFunc);
        }

        private int GetNewSubscriptionId()
        {
            if (!this.recycledSubscriptionIds.TryDequeue(out var result))
            {
                result = this.currentSubscriptionId.Increment();
            }

            return result;
        }

        private void Unsubscribe(int subscriptionId)
        {
            this.currentSubscriptionId.Update(
                v =>
                    {
                        if (this.subscriptions.TryRemove(subscriptionId, out _))
                        {
                            if (subscriptionId == v)
                            {
                                --v;
                            }
                            else
                            {
                                this.recycledSubscriptionIds.Enqueue(subscriptionId);
                            }

                            lock (this.lockObject)
                            {
                                this.subscriptionCache = this.subscriptions.Values;
                            }
                        }

                        return v;
                    });
        }

        private struct ConcurrentMessageBusSubscription : IMessageBusSubscription
        {
            private Action unsubscribeAction;

            public ConcurrentMessageBusSubscription(Action unsubscribeAction)
            {
                this.unsubscribeAction = unsubscribeAction;
            }

            public void Unsubscribe()
            {
                this.unsubscribeAction.Invoke();
                this.unsubscribeAction = ActionHelpers.NoAction;
            }

            public void Dispose()
            {
                this.Unsubscribe();
            }
        }

        private struct StrongReferenceSubscription : ISubscription<TMessageType>
        {
            public StrongReferenceSubscription(Func<TMessageType, Task> subscriptionHandlerFunc)
            {
                this.SubscriptionHandlerFunc = subscriptionHandlerFunc;
            }

            public Func<TMessageType, Task> SubscriptionHandlerFunc { get; set; }
        }

        private struct WeakReferenceSubscription : ISubscription<TMessageType>
        {
            private readonly WeakReference<Func<TMessageType, Task>> subscriptionHandlerFunc;

            public WeakReferenceSubscription(Func<TMessageType, Task> subscriptionHandlerFunc)
            {
                this.subscriptionHandlerFunc = new WeakReference<Func<TMessageType, Task>>(subscriptionHandlerFunc);
            }

            public Func<TMessageType, Task> SubscriptionHandlerFunc
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
    }
}