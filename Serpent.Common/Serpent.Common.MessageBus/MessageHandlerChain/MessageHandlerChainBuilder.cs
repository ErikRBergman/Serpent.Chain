// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The message handler chain builder. Used to create decorator and handler chains
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct MessageHandlerChainBuilder<TMessageType> : IMessageHandlerChainBuilder<TMessageType>
    {
        private readonly IMessageBusSubscriptions<TMessageType> subscriptions;

        private Stack<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>> handlerSetupFuncs;

        /// <summary>
        /// Creates a new instance of the MessageHandlerChainBuilder
        /// </summary>
        /// <param name="subscriptions">The subscriptions interface used to subscribe when the chain is terminated</param>
        public MessageHandlerChainBuilder(IMessageBusSubscriptions<TMessageType> subscriptions)
        {
            this.subscriptions = subscriptions;
            this.handlerSetupFuncs = new Stack<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>>();
        }

        /// <summary>
        /// The number of handler setup methods
        /// </summary>
        public int Count => this.handlerSetupFuncs?.Count ?? 0;

        /// <summary>
        /// Adds a method that sets up the message decorator
        /// </summary>
        /// <param name="addFunc">
        /// The method that sets up a message decorator
        /// </param>
        /// <returns>
        /// The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;"/>.
        /// </returns>
        public IMessageHandlerChainBuilder<TMessageType> Add(Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> addFunc)
        {
            if (this.handlerSetupFuncs == null)
            {
                this.handlerSetupFuncs = new Stack<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>>();
            }

            this.handlerSetupFuncs.Push(addFunc);
            return this;
        }

        /// <summary>
        /// Creates the subscription builds the message handler chain
        /// </summary>
        /// <param name="handlerFunc">The handler function
        /// </param>
        /// <returns>
        /// The <see cref="IMessageBusSubscription"/>.
        /// </returns>
        public IMessageBusSubscription Handler(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.subscriptions.Subscribe(this.Build(handlerFunc));
        }

        /// <summary>
        /// Builds the message handler chain
        /// </summary>
        /// <param name="handlerFunc">
        /// The handler that receives the message when all decorators have run
        /// </param>
        /// <returns>
        /// The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;"/>.
        /// </returns>
        public Func<TMessageType, CancellationToken, Task> Build(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            if (this.handlerSetupFuncs == null || this.handlerSetupFuncs.Count == 0)
            {
                return handlerFunc;
            }

            return this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current));
        }
    }
}