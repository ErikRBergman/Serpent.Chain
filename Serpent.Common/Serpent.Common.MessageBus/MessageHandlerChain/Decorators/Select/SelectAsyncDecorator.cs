namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Select
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SelectAsyncDecorator<TOldMessageType, TNewMessageType> : IMessageBusSubscriptions<TNewMessageType>
    {
        private readonly IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder;

        private readonly Func<TOldMessageType, Task<TNewMessageType>> selector;

        public SelectAsyncDecorator(IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder, Func<TOldMessageType, Task<TNewMessageType>> selector)
        {
            this.outerMessageHandlerChainBuilder = outerMessageHandlerChainBuilder;
            this.selector = selector;
            this.NewMessageHandlerChainBuilder = new MessageHandlerChainBuilder<TNewMessageType>(this);
        }

        public IMessageHandlerChainBuilder<TNewMessageType> NewMessageHandlerChainBuilder { get; }

        public IMessageBusSubscription Subscribe(Func<TNewMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.outerMessageHandlerChainBuilder.Handler(async (message, token) =>
                {
                    await handlerFunc(await this.selector(message).ConfigureAwait(false), token).ConfigureAwait(false);
                });
        }
    }
}