namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Select
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SelectDecorator<TOldMessageType, TNewMessageType> : IMessageBusSubscriptions<TNewMessageType>
    {
        private readonly IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder;

        private readonly Func<TOldMessageType, TNewMessageType> selector;

        public SelectDecorator(IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder, Func<TOldMessageType, TNewMessageType> selector)
        {
            this.outerMessageHandlerChainBuilder = outerMessageHandlerChainBuilder;
            this.selector = selector;
            this.NewMessageHandlerChainBuilder = new MessageHandlerChainBuilder<TNewMessageType>(this);
        }

        public IMessageHandlerChainBuilder<TNewMessageType> NewMessageHandlerChainBuilder { get; }

        public IMessageBusSubscription Subscribe(Func<TNewMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.outerMessageHandlerChainBuilder.Handler((message, token) => handlerFunc(this.selector(message), token));
        }
    }
}