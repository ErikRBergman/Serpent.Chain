﻿// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class MessageHandlerChainBuilderExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Add<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<Func<TMessageType, Task>, BusSubscription<TMessageType>> addFunc)
        {
            return messageHandlerChainBuilder.Add(previousHandler => addFunc(previousHandler).HandleMessageAsync);
        }

        public static IMessageHandlerChainBuilder<TMessageType> Subscribe<TMessageType>(this IMessageBusSubscriber<TMessageType> subscriber)
        {
            return new MessageHandlerChainBuilder<TMessageType>(subscriber);
        }
    }
}