// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class SemapshoreExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Semaphore<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, int maxNumberOfConcurrentMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new SemaphoreDecorator<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }
    }
}