namespace Serpent.Common.MessageBus.MessageHandlerChain
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

        public IMessageBusSubscription Subscribe(Func<TNewMessageType, CancellationToken, Task> invocationFunc)
        {
            return this.outerMessageHandlerChainBuilder.Handler((message, token) => invocationFunc(this.selector(message), token));
        }
    }
}