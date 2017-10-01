// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class MessageHandlerChainBuilderExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Add<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainDecorator<TMessageType>> addFunc)
        {
            return messageHandlerChainBuilder.Add(previousHandler => addFunc(previousHandler).HandleMessageAsync);
        }

        public static IMessageBusSubscription Handler<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Action<TMessageType> handlerFunc)
        {
            return messageHandlerChainBuilder.Handler(
                (message, token) =>
                    {
                        handlerFunc(message);
                        return Task.CompletedTask;
                    });
        }

        public static IMessageBusSubscription Handler<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, Task> handlerFunc)
        {
            return messageHandlerChainBuilder.Handler(
                (message, token) => handlerFunc(message));
        }

        public static IMessageBusSubscription Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            IMessageHandler<TMessageType> handler)
        {
            return messageHandlerChainBuilder.Handler(handler.HandleMessageAsync);
        }

        public static IMessageHandlerChainBuilder<TMessageType> Subscribe<TMessageType>(this IMessageBusSubscriptions<TMessageType> subscriptions)
        {
            return new MessageHandlerChainBuilder<TMessageType>(subscriptions);
        }
    }
}