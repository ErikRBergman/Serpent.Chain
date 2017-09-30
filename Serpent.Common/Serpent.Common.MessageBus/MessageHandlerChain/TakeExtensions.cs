// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class TakeExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Take<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int numberOfMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new TakeDecorator<TMessageType>(currentHandler, numberOfMessages));
        }
    }
}