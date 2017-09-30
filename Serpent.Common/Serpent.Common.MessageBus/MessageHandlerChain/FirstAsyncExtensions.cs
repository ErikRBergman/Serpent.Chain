// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class FirstAsyncExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> FirstAsync<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> asyncPredicate)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FirstAsyncDecorator<TMessageType>(currentHandler, asyncPredicate));
        }

    }
}