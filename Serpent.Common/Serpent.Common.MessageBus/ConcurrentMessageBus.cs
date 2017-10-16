namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Helpers;
    using Serpent.Common.MessageBus.Interfaces;

    /// <summary>
    ///     The message bus
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class ConcurrentMessageBus<TMessageType> : IMessageBus<TMessageType>
    {
        private readonly ExclusiveAccess<int> currentSubscriptionId = new ExclusiveAccess<int>(0);

        private readonly object subscriptionsCacheLock = new object();

        private readonly ConcurrentMessageBusOptions<TMessageType> options = ConcurrentMessageBusOptions<TMessageType>.Default;

        private readonly Func<IEnumerable<IMessageHandler<TMessageType>>, TMessageType, CancellationToken, Task> publishAsyncFunc;

        private readonly ConcurrentQueue<int> recycledSubscriptionIds = new ConcurrentQueue<int>();

        private readonly ConcurrentDictionary<int, IMessageHandler<TMessageType>> subscriptions = new ConcurrentDictionary<int, IMessageHandler<TMessageType>>();

        private IEnumerable<IMessageHandler<TMessageType>> subscriptionCache = Array.Empty<IMessageHandler<TMessageType>>();

        /// <summary>
        ///     Creates a new instance of the message bus, providing an options object
        /// </summary>
        /// <param name="options">The options</param>
        public ConcurrentMessageBus(ConcurrentMessageBusOptions<TMessageType> options)
        {
            this.options = options;
            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        /// <summary>
        ///     Creates a new instance of the message bus, providing an options object
        /// </summary>
        /// <param name="optionsAction">A method that configures the message bus options</param>
        public ConcurrentMessageBus(Action<ConcurrentMessageBusOptions<TMessageType>> optionsAction)
        {
            var newOptions = new ConcurrentMessageBusOptions<TMessageType>();
            optionsAction(newOptions);
            this.options = newOptions;
            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        /// <summary>
        ///     Creates a new instance of the message bus, providing an options object
        /// </summary>
        public ConcurrentMessageBus()
        {
            if (this.options.BusPublisher == null)
            {
                throw new Exception("No BusPublisher must not be null");
            }

            this.publishAsyncFunc = this.options.BusPublisher.PublishAsync;
        }

        /// <summary>
        ///     The current number of subscribers
        /// </summary>
        public int SubscriberCount => this.subscriptions.Count;

        /// <summary>
        ///     Publishes a messages to the message bus, returning a Task that is done when the message is handled
        /// </summary>
        /// <param name="message">
        ///     The messgae to publish
        /// </param>
        /// <param name="token">
        ///     A cancellation token that can be used to cancel handling the message
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task PublishAsync(TMessageType message, CancellationToken token)
        {
            return this.publishAsyncFunc(this.subscriptionCache, message, token);
        }

        /// <summary>
        ///     Subscribes to the message bus with the specified method handling all messagess
        /// </summary>
        /// <param name="handlerFunc">
        ///     The handler
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageBusSubscription" /> used to unsubscribe.
        /// </returns>
        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            var subscription = this.CreateSubscription(handlerFunc);

            var newSubscriptionId = this.GetNewSubscriptionId();

            this.subscriptions.TryAdd(newSubscriptionId, subscription);

            lock (this.subscriptionsCacheLock)
            {
                this.subscriptionCache = this.subscriptions.Values;
            }

            return this.CreateMessageBusSubscription(newSubscriptionId);
        }

        private ConcurrentMessageBusSubscription CreateMessageBusSubscription(int newSubscriptionId)
        {
            return new ConcurrentMessageBusSubscription(() => this.Unsubscribe(newSubscriptionId));
        }

        private IMessageHandler<TMessageType> CreateSubscription(Func<TMessageType, CancellationToken, Task> subscriptionHandlerFunc)
        {
            return this.options.SubscriptionReferenceType != SubscriptionReferenceTypeType.WeakReferences
                       ? new StrongReferenceHandler<TMessageType>(subscriptionHandlerFunc)
                       : (IMessageHandler<TMessageType>)new WeakReferenceHandler<TMessageType>(subscriptionHandlerFunc);
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

                            lock (this.subscriptionsCacheLock)
                            {
                                this.subscriptionCache = this.subscriptions.Values;
                            }
                        }

                        return v;
                    });
        }
    }
}