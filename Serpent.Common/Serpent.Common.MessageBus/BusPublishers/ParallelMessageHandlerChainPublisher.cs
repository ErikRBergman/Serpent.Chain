// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Models;

    public class ParallelMessageHandlerChainPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<MessageAndSubscription<TMessageType>, CancellationToken, Task> publisher;

        public ParallelMessageHandlerChainPublisher(MessageHandlerChainBuilder<MessageAndSubscription<TMessageType>> messageHandlerChainBuilder)
        {
            this.publisher = messageHandlerChainBuilder.Build(this.PublishAsync);
        }

        public ParallelMessageHandlerChainPublisher(Func<MessageAndSubscription<TMessageType>, CancellationToken, Task> handlerFunc)
        {
            this.publisher = handlerFunc;
        }

        public override Task PublishAsync(IEnumerable<ISubscription<TMessageType>> subscriptions, TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(subscriptions.Select(subscription => this.publisher(new MessageAndSubscription<TMessageType>(message, subscription), token)));
        }

        private Task PublishAsync(MessageAndSubscription<TMessageType> messageAndSubscription, CancellationToken token)
        {
            var subscriptionHandlerFunc = messageAndSubscription.Subscription.SubscriptionHandlerFunc;
            
            // subscriptionHandlerFunc is null when refering to a weak reference that has been garbage collected
            return subscriptionHandlerFunc == null ? Task.CompletedTask : subscriptionHandlerFunc(messageAndSubscription.Message, token);
        }
    }
}