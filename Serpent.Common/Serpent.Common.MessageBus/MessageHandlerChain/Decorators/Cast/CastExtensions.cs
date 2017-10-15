// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    public static class CastExtensions
    {
        public static IMessageHandlerChainBuilder<TNewType> Cast<TMessageType, TNewType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
            where TNewType : class
        {
            return messageHandlerChainBuilder.Select(m => m as TNewType);
        }
    }
}