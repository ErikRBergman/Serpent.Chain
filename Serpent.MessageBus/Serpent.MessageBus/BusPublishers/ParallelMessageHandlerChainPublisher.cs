// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain;
    using Serpent.MessageBus.Models;

    /// <summary>
    /// The parallel message handler chain publisher.
    /// Used to publish messages through a message handler chain
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class ParallelMessageHandlerChainPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<MessageAndHandler<TMessageType>, CancellationToken, Task> publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelMessageHandlerChainPublisher{TMessageType}"/> class. 
        /// </summary>
        /// <param name="messageHandlerChainBuilder">
        /// The message handler chain builder
        /// </param>
        public ParallelMessageHandlerChainPublisher(MessageHandlerChainBuilder<MessageAndHandler<TMessageType>> messageHandlerChainBuilder)
        {
            messageHandlerChainBuilder.Handler(this.PublishAsync);

            var subscriptionNotification = new MessageHandlerChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);
            this.publisher = messageHandlerChainBuilder.BuildFunc(services);

            var chain = new MessageHandlerChain<MessageAndHandler<TMessageType>>(this.publisher);
            subscriptionNotification.Notify(chain);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelMessageHandlerChainPublisher{TMessageType}"/> class. 
        /// </summary>
        /// <param name="handlerFunc">
        /// The handler Func.
        /// </param>
        public ParallelMessageHandlerChainPublisher(Func<MessageAndHandler<TMessageType>, CancellationToken, Task> handlerFunc)
        {
            this.publisher = handlerFunc;
        }

        /// <summary>
        /// Publishes a message
        /// </summary>
        /// <param name="handlers">The message handlers handlers</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A <see cref="Task"/> that will complete when all messages are handled</returns>
        public override Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken cancellationToken)
        {
            return Task.WhenAll(handlers.Select(subscription => this.publisher(new MessageAndHandler<TMessageType>(message, subscription), cancellationToken)));
        }

        private Task PublishAsync(MessageAndHandler<TMessageType> messageAndHandler, CancellationToken token)
        {
            return messageAndHandler.MessageHandler(messageAndHandler.Message, token);
        }
    }
}