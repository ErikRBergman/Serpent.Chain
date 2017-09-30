// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class BranchExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Branch<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new BranchDecorator<TMessageType>(currentHandler, branches));
        }
    }
}