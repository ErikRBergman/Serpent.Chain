// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class SoftFireAndForgetExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> SoftFireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new SoftFireAndForgetDecorator<TMessageType>(currentHandler).HandleMessageAsync);
        }
    }
}