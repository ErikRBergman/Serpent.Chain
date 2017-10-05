// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Skip;

    public static class SkipExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Skip<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int numberOfMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new SkipDecorator<TMessageType>(currentHandler, numberOfMessages));
        }
    }
}