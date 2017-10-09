// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct;
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public static class DistinctExtensions
    {
        [ExtensionMethodSelector("DistinctWireUp")]
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, TKeyType> keySelector)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctDecorator<TMessageType, TKeyType>(currentHandler, keySelector, equalityComparer));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, Task<TKeyType>> keySelector)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, (message, token) => keySelector(message)));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, (message, token) => keySelector(message), equalityComparer));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, keySelector, equalityComparer));
        }
    }
}