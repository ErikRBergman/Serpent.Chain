// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    public static class OfTypeExtensions
    {
        public static IMessageHandlerChainBuilder<TNewType> OfType<TMessageType, TNewType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
            where TNewType : class, TMessageType
        {
            return messageHandlerChainBuilder.Where(m => m is TNewType).Select(m => m as TNewType);
        }
    }
}