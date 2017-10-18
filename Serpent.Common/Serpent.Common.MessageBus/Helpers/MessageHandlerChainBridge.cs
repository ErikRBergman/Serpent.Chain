namespace Serpent.Common.MessageBus.Helpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class MessageHandlerChainBridge<TMessageType> : IMessageBusSubscriptions<TMessageType>
    {
        private readonly IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder;

        public MessageHandlerChainBridge(IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            this.messageHandlerChainBuilder = messageHandlerChainBuilder;
        }

        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            return this.messageHandlerChainBuilder.Handler(handlerFunc);
        }
    }
}