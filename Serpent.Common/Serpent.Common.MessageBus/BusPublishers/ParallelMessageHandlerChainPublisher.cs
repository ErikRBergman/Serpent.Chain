// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.Models;

    public class ParallelMessageHandlerChainPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<MessageAndHandler<TMessageType>, CancellationToken, Task> publisher;

        public ParallelMessageHandlerChainPublisher(MessageHandlerChainBuilder<MessageAndHandler<TMessageType>> messageHandlerChainBuilder)
        {
            this.publisher = messageHandlerChainBuilder.Build(this.PublishAsync);
        }

        public ParallelMessageHandlerChainPublisher(Func<MessageAndHandler<TMessageType>, CancellationToken, Task> handlerFunc)
        {
            this.publisher = handlerFunc;
        }

        public override Task PublishAsync(IEnumerable<IMessageHandler<TMessageType>> subscriptions, TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(subscriptions.Select(subscription => this.publisher(new MessageAndHandler<TMessageType>(message, subscription), token)));
        }

        private Task PublishAsync(MessageAndHandler<TMessageType> messageAndHandler, CancellationToken token)
        {
            return messageAndHandler.Subscription.HandleMessageAsync(messageAndHandler.Message, token);
        }
    }
}