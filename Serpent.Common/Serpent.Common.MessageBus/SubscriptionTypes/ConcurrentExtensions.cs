// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class ConcurrentExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Concurrent<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, int maxNumberOfConcurrentMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new ConcurrentDecorator<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }

        public static IMessageHandlerChainBuilder<TMessageType> ConcurrentFireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, int maxNumberOfConcurrentMessages, bool fireAndForget = false)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new ConcurrentFireAndForgetDecorator<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }
    }
}