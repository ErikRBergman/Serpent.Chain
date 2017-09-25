// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class DelayExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Delay<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, TimeSpan timeToWait, bool dontAwait = false)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DelaySubscription<TMessageType>(currentHandler, timeToWait, dontAwait).HandleMessageAsync);
        }

        public static IMessageHandlerChainBuilder<TMessageType> Delay<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int timeInMilliseconds,
            bool dontAwait = false)
        {
            return messageHandlerChainBuilder.Add(
                currentHandler => new DelaySubscription<TMessageType>(currentHandler, TimeSpan.FromMilliseconds(timeInMilliseconds), dontAwait).HandleMessageAsync);
        }
    }
}