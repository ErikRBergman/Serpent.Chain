// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Exceptions;
    using Serpent.MessageBus.MessageHandlerChain;

    /// <summary>
    ///     The message handler chain builder. Used to create decorator and handler chains
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class MessageHandlerChainBuilder<TMessageType> : IMessageHandlerChainBuilder<TMessageType>
    {
        private readonly Stack<Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>>> handlerSetupFuncs =
                new Stack<Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>>>();

        private Func<MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> createHandlerFunc = null;

        /// <summary>
        ///     The number of handler setup methods
        /// </summary>
        public int Count => this.handlerSetupFuncs?.Count ?? 0;
        
        /// <summary>
        /// Returns true if the builder has a handler
        /// </summary>
        public bool HasHandler => this.createHandlerFunc != null;


        /// <summary>
        ///     Adds a decorator to the message handler chain
        /// </summary>
        /// <param name="addFunc">
        ///     The method that sets up a message decorator
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;" />.
        /// </returns>
        public IMessageHandlerChainBuilder<TMessageType> Decorate(
            Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addFunc)
        {
            if (this.createHandlerFunc != null)
            {
                throw new MessageHandlerChainHasAHandlerException("The message handler chain has a handler. Decorators can not be added when then chain has a handler.");
            }

            this.handlerSetupFuncs.Push(addFunc);
            return this;
        }

        /// <summary>
        ///     Adds a handler to the message handler chain. When a handler is added, no more decorators or handlers can be added
        /// </summary>
        /// <param name="addHandlerFunc">The method called to create and return the message handler chain's message handler</param>
        public IMessageHandlerChainBuilder<TMessageType> Handle(Func<MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addHandlerFunc)
        {
            if (this.createHandlerFunc != null)
            {
                throw new MessageHandlerChainHasAHandlerException("The message handler chain already has a handler. A message handler chain can only have a single handler.");
            }

            this.createHandlerFunc = addHandlerFunc;

            return this;
        }

        /// <summary>
        ///     Builds a message handler chain from the decorators and the handler added
        /// </summary>
        /// <returns>
        ///     The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;" />.
        /// </returns>
        public IMessageHandlerChain<TMessageType> BuildChain()
        {
            if (this.createHandlerFunc == null)
            {
                throw new MessageHandlerChainHasNoMessageHandlerException("The message handler chain does not have a message handler");
            }

            var subscriptionNotification = new MessageHandlerChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);

            var handlerFunc = this.createHandlerFunc(services);

            if (this.handlerSetupFuncs == null || this.handlerSetupFuncs.Count == 0)
            {
                return new MessageHandlerChain<TMessageType>(handlerFunc);
            }

            var func = this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current, services));

            return new MessageHandlerChain<TMessageType>(func);
        }

        /// <summary>
        /// Builds the message handler chain
        /// </summary>
        /// <param name="services">
        /// The setup notification services.
        /// </param>
        /// <returns>
        /// The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;"/>.
        /// </returns>
        public Func<TMessageType, CancellationToken, Task> BuildFunc(MessageHandlerChainBuilderSetupServices services)
        {
            if (this.createHandlerFunc == null)
            {
                throw new MessageHandlerChainHasNoMessageHandlerException("The message handler chain does not have a message handler");
            }

            var handlerFunc = this.createHandlerFunc(services);

            if (this.handlerSetupFuncs == null || this.handlerSetupFuncs.Count == 0)
            {
                return handlerFunc;
            }

            return this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current, services));
        }
    }
}