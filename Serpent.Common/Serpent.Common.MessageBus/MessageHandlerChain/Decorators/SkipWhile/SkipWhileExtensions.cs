// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class SkipWhileExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> SkipWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate,
            int numberOfMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new SkipWhileDecorator<TMessageType>(currentHandler, predicate));
        }

        public static IMessageHandlerChainBuilder<TMessageType> SkipWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> predicate,
            int numberOfMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new SkipWhileAsyncDecorator<TMessageType>(currentHandler, predicate));
        }
    }
}