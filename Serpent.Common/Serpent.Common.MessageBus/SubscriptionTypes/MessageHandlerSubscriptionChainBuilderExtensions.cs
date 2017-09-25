namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public static class MessageHandlerSubscriptionChainBuilderExtensions
    {
        public static IMessageBusSubscription Handler<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Action<TMessageType> handlerFunc)
        {
            return messageHandlerChainBuilder.Handler(
                message =>
                    {
                        handlerFunc(message);
                        return Task.CompletedTask;
                    });
        }

        public static IMessageBusSubscription Handler<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, IMessageHandler<TMessageType> handler)
        {
            return messageHandlerChainBuilder.Handler(handler.HandleMessageAsync);
        }


    }
}