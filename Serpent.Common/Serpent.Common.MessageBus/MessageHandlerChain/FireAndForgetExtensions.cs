// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class FireAndForgetExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> FireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FireAndForgetDecorator<TMessageType>(currentHandler).HandleMessageAsync);
        }
    }
}