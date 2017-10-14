namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SelectMany
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SelectManyAsyncDecorator<TOldMessageType, TNewMessageType> : IMessageBusSubscriptions<TNewMessageType>
    {
        private readonly IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder;

        private readonly Func<TOldMessageType, Task<IEnumerable<TNewMessageType>>> selector;

        public SelectManyAsyncDecorator(IMessageHandlerChainBuilder<TOldMessageType> outerMessageHandlerChainBuilder, Func<TOldMessageType, Task<IEnumerable<TNewMessageType>>> selector)
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
                    var messages = await this.selector(message).ConfigureAwait(false);
                    foreach (var msg in messages)
                    {
                        await handlerFunc(msg, token).ConfigureAwait(false);
                    }
                });
        }
    }
}