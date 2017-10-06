// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public static class WireUpExtension
    {
        public static IMessageHandlerChainBuilder<TMessageType> WireUp<TMessageType, TWireUpType, THandlerType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            return WireUpMap.Default.WireUpHandler<TMessageType, TWireUpType, THandlerType>(messageHandlerChainBuilder, handler);
        }

        public static IMessageHandlerChainBuilder<TMessageType> WireUp<TMessageType, THandlerType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            return WireUpMap.Default.WireUpHandler<TMessageType, THandlerType, THandlerType>(messageHandlerChainBuilder, handler);
        }
    }
}