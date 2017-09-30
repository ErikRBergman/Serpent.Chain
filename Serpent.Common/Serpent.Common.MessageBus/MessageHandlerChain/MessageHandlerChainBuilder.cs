// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public struct MessageHandlerChainBuilder<TMessageType> : IMessageHandlerChainBuilder<TMessageType>
    {
        private readonly IMessageBusSubscriber<TMessageType> subscriber;

        private Stack<Func<Func<TMessageType, Task>, Func<TMessageType, Task>>> handlerSetupFuncs;

        public MessageHandlerChainBuilder(IMessageBusSubscriber<TMessageType> subscriber)
        {
            this.subscriber = subscriber;
            this.handlerSetupFuncs = new Stack<Func<Func<TMessageType, Task>, Func<TMessageType, Task>>>();
        }

        public int Count => this.handlerSetupFuncs?.Count ?? 0;

        public IMessageHandlerChainBuilder<TMessageType> Add(Func<Func<TMessageType, Task>, Func<TMessageType, Task>> addFunc)
        {
            if (this.handlerSetupFuncs == null)
            {
                this.handlerSetupFuncs = new Stack<Func<Func<TMessageType, Task>, Func<TMessageType, Task>>>();
            }

            this.handlerSetupFuncs.Push(addFunc);
            return this;
        }

        public IMessageBusSubscription Factory<THandler>(Func<THandler> handlerFactory)
            where THandler : IMessageHandler<TMessageType>
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(THandler)))
            {
                return this.subscriber.Subscribe(
                    this.Build(
                        async message =>
                            {
                                var handler = handlerFactory();
                                try
                                {
                                    await handler.HandleMessageAsync(message);
                                }
                                finally
                                {
                                    ((IDisposable)handler).Dispose();
                                }
                            }));
            }

            return this.subscriber.Subscribe(
                this.Build(
                    message =>
                        {
                            var handler = handlerFactory();
                            return handler.HandleMessageAsync(message);
                        }));
        }

        public IMessageBusSubscription Handler(Func<TMessageType, Task> handlerFunc)
        {
            return this.subscriber.Subscribe(this.Build(handlerFunc));
        }

        internal Func<TMessageType, Task> Build(Func<TMessageType, Task> handlerFunc)
        {
            if (this.handlerSetupFuncs == null || this.handlerSetupFuncs.Count == 0)
            {
                return handlerFunc;
            }

            return this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current));
        }
    }
}