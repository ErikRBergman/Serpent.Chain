// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.TakeWhile;

    public static class TakeWhileExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> TakeWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate,
            int numberOfMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new TakeWhileDecorator<TMessageType>(currentHandler, predicate));
        }

        public static IMessageHandlerChainBuilder<TMessageType> TakeWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> predicate,
            int numberOfMessages)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new TakeWhileAsyncDecorator<TMessageType>(currentHandler, predicate));
        }
    }
}