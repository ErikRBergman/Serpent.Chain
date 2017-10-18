// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    /// <summary>
    ///     The message handler chain builder. Used to create decorator and handler chains
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct MessageHandlerChainBuilder<TMessageType> : IMessageHandlerChainBuilder<TMessageType>
    {
        private Stack<Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>>> handlerSetupFuncs;

        /// <summary>
        ///     Creates a new instance of the MessageHandlerChainBuilder
        /// </summary>
        /// <param name="subscriptions">The subscriptions interface used to subscribe when the chain is terminated</param>
        public MessageHandlerChainBuilder(IMessageBusSubscriptions<TMessageType> subscriptions)
        {
            this.MessageBusSubscriptions = subscriptions;
            this.handlerSetupFuncs =
                new Stack<Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>>>();
        }

        /// <summary>
        ///     The number of handler setup methods
        /// </summary>
        public int Count => this.handlerSetupFuncs?.Count ?? 0;

        /// <summary>
        ///     The message bus subscriptions
        /// </summary>
        public IMessageBusSubscriptions<TMessageType> MessageBusSubscriptions { get; }

        /// <summary>
        ///     Adds a method that sets up the message decorator
        /// </summary>
        /// <param name="addFunc">
        ///     The method that sets up a message decorator
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;" />.
        /// </returns>
        public IMessageHandlerChainBuilder<TMessageType> Add(
            Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addFunc)
        {
            if (this.handlerSetupFuncs == null)
            {
                this.handlerSetupFuncs =
                    new Stack<Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>>>();
            }

            this.handlerSetupFuncs.Push(addFunc);
            return this;
        }

        /// <summary>
        ///     Builds the message handler chain
        /// </summary>
        /// <param name="handlerFunc">
        ///     The handler that receives the message when all decorators have run
        /// </param>
        /// <returns>
        ///     The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;" />.
        /// </returns>
        public Func<TMessageType, CancellationToken, Task> Build(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            if (this.handlerSetupFuncs == null || this.handlerSetupFuncs.Count == 0)
            {
                return handlerFunc;
            }

            var subscriptionNotification = new MessageHandlerChainSubscriptionNotification();
            var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);

            return this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current, services));
        }

        /// <summary>
        ///     Builds the message handler chain and creates the subscription
        /// </summary>
        /// <param name="handlerFunc">
        ///     The handler function
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageBusSubscription" />.
        /// </returns>
        public IMessageBusSubscription Handler(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            // Setup builder services
            var subscriptionNotification = new MessageHandlerChainSubscriptionNotification();
            var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);

            var subscription = this.MessageBusSubscriptions.Subscribe(this.Build(handlerFunc, services));

            // notify all interested parties of the subscription
            subscriptionNotification.Notify(subscription);

            return subscription;
        }

        private Func<TMessageType, CancellationToken, Task> Build(Func<TMessageType, CancellationToken, Task> handlerFunc, MessageHandlerChainBuilderSetupServices services)
        {
            if (this.handlerSetupFuncs == null || this.handlerSetupFuncs.Count == 0)
            {
                return handlerFunc;
            }

            return this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current, services));
        }
    }
}