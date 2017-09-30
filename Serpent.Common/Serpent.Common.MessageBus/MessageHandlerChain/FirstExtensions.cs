// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class FirstExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> First<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FirstDecorator<TMessageType>(currentHandler, predicate));
        }

        public static IMessageHandlerChainBuilder<TMessageType> First<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FirstDecorator<TMessageType>(currentHandler, msg => true));
        }

        public static IMessageHandlerChainBuilder<TMessageType> First<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> asyncPredicate)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FirstAsyncDecorator<TMessageType>(currentHandler, asyncPredicate));
        }
    }
}