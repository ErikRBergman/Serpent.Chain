// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public struct MessageHandlerChainBuilder<TMessageType> : IMessageHandlerChainBuilder<TMessageType>
    {
        private readonly IMessageBusSubscriptions<TMessageType> subscriptions;

        private Stack<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>> handlerSetupFuncs;

        public MessageHandlerChainBuilder(IMessageBusSubscriptions<TMessageType> subscriptions)
        {
            this.subscriptions = subscriptions;
            this.handlerSetupFuncs = new Stack<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>>();
        }

        public int Count => this.handlerSetupFuncs?.Count ?? 0;

        public IMessageHandlerChainBuilder<TMessageType> Add(Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> addFunc)
        {
            if (this.handlerSetupFuncs == null)
            {
                this.handlerSetupFuncs = new Stack<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>>();
            }

            this.handlerSetupFuncs.Push(addFunc);
            return this;
        }

        public IMessageBusSubscription Factory<THandler>(Func<THandler> handlerFactory)
            where THandler : IMessageHandler<TMessageType>
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(THandler)))
            {
                return this.subscriptions.Subscribe(
                    this.Build(
                        async (message, token) =>
                            {
                                var handler = handlerFactory();
                                try
                                {
                                    await handler.HandleMessageAsync(message, token);
                                }
                                finally
                                {
                                    ((IDisposable)handler).Dispose();
                                }
                            }));
            }

            return this.subscriptions.Subscribe(
                this.Build(
                    (message, token) =>
                        {
                            var handler = handlerFactory();
                            return handler.HandleMessageAsync(message, token);
                        }));
        }

        public IMessageBusSubscription Handler(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.subscriptions.Subscribe(this.Build(handlerFunc));
        }

        internal Func<TMessageType, CancellationToken, Task> Build(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            if (this.handlerSetupFuncs == null || this.handlerSetupFuncs.Count == 0)
            {
                return handlerFunc;
            }

            return this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current));
        }
    }
}