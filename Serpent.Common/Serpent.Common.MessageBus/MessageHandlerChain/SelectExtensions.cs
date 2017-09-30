// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class SelectExtensions
    {
        public static IMessageHandlerChainBuilder<TNewMessageType> Select<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TNewMessageType> selector)
        {
            return new SelectDecorator<TMessageType, TNewMessageType>(messageHandlerChainBuilder, selector).NewMessageHandlerChainBuilder;
        }

        public static IMessageHandlerChainBuilder<TNewMessageType> Select<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<TNewMessageType>> selector)
        {
            return new SelectAsyncDecorator<TMessageType, TNewMessageType>(messageHandlerChainBuilder, selector).NewMessageHandlerChainBuilder;
        }
    }
}