namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading.Tasks;

    public class SelectAsyncDecorator<TOldMessageType, TNewMessageType> : IMessageBusSubscriber<TNewMessageType>
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

        public IMessageBusSubscription Subscribe(Func<TNewMessageType, Task> invocationFunc)
        {
            return this.outerMessageHandlerChainBuilder.Handler(async message =>
                {
                    await invocationFunc(await this.selector(message).ConfigureAwait(false)).ConfigureAwait(false);
                });
        }
    }
}