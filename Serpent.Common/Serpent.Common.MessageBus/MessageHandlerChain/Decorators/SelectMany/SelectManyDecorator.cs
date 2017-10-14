namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SelectMany
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SelectManyDecorator<TOldMessageType, TNewMessageType> : IMessageBusSubscriptions<TNewMessageType>
    {
        private readonly IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder;

        private readonly Func<TOldMessageType, IEnumerable<TNewMessageType>> selector;

        public SelectManyDecorator(IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder, Func<TOldMessageType, IEnumerable<TNewMessageType>> selector)
        {
            this.outerMessageHandlerChainBuilder = outerMessageHandlerChainBuilder;
            this.selector = selector;
            this.NewMessageHandlerChainBuilder = new MessageHandlerChainBuilder<TNewMessageType>(this);
        }

        public IMessageHandlerChainBuilder<TNewMessageType> NewMessageHandlerChainBuilder { get; }

        public IMessageBusSubscription Subscribe(Func<TNewMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.outerMessageHandlerChainBuilder.Handler((message, token) =>
                {
                    var messages = this.selector(message);
                    return Task.WhenAll(messages.Select(msg => handlerFunc(msg, token)));
                });
        }
    }
}