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
    }
}