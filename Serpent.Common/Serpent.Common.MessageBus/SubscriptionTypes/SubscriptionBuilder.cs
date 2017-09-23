// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public struct SubscriptionBuilder<TMessageType>
    {
        private readonly IMessageBusSubscriber<TMessageType> subscriber;

        private Stack<Func<Func<TMessageType, Task>, Func<TMessageType, Task>>> handlerSetupFuncs;

        public SubscriptionBuilder(IMessageBusSubscriber<TMessageType> subscriber)
        {
            this.subscriber = subscriber;
            this.handlerSetupFuncs = new Stack<Func<Func<TMessageType, Task>, Func<TMessageType, Task>>>();
        }

        public SubscriptionBuilder<TMessageType> Add(Func<Func<TMessageType, Task>, Func<TMessageType, Task>> addFunc)
        {
            this.handlerSetupFuncs.Push(addFunc);
            return this;
        }

        public IMessageBusSubscription Handler(Func<TMessageType, Task> handlerFunc)
        {
            return this.subscriber.Subscribe(this.Build(handlerFunc));
        }

        public IMessageBusSubscription Handler(Action<TMessageType> handlerAction)
        {
            return this.subscriber.Subscribe(this.Build(
                message =>
                    {
                        handlerAction(message);
                        return Task.CompletedTask;
                    }));
        }

        public IMessageBusSubscription Handler(IMessageHandler<TMessageType> handler)
        {
            return this.subscriber.Subscribe(this.Build(handler.HandleMessageAsync));
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

        internal Func<TMessageType, Task> Build(Func<TMessageType, Task> handlerFunc)
        {
            return this.handlerSetupFuncs.Aggregate(handlerFunc, (current, handlerSetupFunc) => handlerSetupFunc(current));
        }
    }
}