// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SelectMany;

    public static class SelectManyExtensions
    {
        public static IMessageHandlerChainBuilder<TNewMessageType> SelectMany<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, IEnumerable<TNewMessageType>> selector)
        {
            return new SelectManyDecorator<TMessageType, TNewMessageType>(messageHandlerChainBuilder, selector).NewMessageHandlerChainBuilder;
        }

        public static IMessageHandlerChainBuilder<TNewMessageType> SelectMany<TMessageType, TNewMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<IEnumerable<TNewMessageType>>> selector)
        {
            return new SelectManyAsyncDecorator<TMessageType, TNewMessageType>(messageHandlerChainBuilder, selector).NewMessageHandlerChainBuilder;
        }
    }
}