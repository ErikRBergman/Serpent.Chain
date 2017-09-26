// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class FireAndForgetExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> FireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FireAndForgetDecorator<TMessageType>(currentHandler).HandleMessageAsync);
        }

        public static IMessageHandlerChainBuilder<TMessageType> SoftFireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FireAndForgetSoftDecorator<TMessageType>(currentHandler).HandleMessageAsync);
        }
    }
}