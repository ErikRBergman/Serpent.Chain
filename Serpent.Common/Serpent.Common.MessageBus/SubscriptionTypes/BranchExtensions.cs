// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class BranchExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Branch<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new BranchSubscription<TMessageType>(currentHandler, branches));
        }
    }
}