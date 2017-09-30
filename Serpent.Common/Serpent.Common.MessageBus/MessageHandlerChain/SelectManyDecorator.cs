namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SelectManyDecorator<TOldMessageType, TNewMessageType> : IMessageBusSubscriber<TNewMessageType>
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

        public IMessageBusSubscription Subscribe(Func<TNewMessageType, Task> invocationFunc)
        {
            return this.outerMessageHandlerChainBuilder.Handler(async message =>
                {
                    var messages = this.selector(message);
                    foreach (var msg in messages)
                    {
                        await invocationFunc(msg);
                    }
                });
        }
    }
}