// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class FireAndForgetExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> FireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new FireAndForgetSubscription<TMessageType>(currentHandler).HandleMessageAsync);
        }
    }
}