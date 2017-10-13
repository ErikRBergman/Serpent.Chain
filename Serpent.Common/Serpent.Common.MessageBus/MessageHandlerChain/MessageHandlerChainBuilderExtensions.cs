// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain;

    /// <summary>
    /// Extensions for IMessageHandlerChainBuilder
    /// </summary>
    public static class MessageHandlerChainBuilderExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Add<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainDecorator<TMessageType>> addFunc)
        {
            return messageHandlerChainBuilder.Add(previousHandler => addFunc(previousHandler).HandleMessageAsync);
        }

        /// <summary>
        /// Set the chain handler method or lambda method. Use this overload if your handler has no need for async or cancellationToken
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="handlerAction">The action to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageBusSubscription Handler<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Action<TMessageType> handlerAction)
        {
            return messageHandlerChainBuilder.Handler(
                (message, token) =>
                    {
                        handlerAction(message);
                        return Task.CompletedTask;
                    });
        }


        /// <summary>
        /// Set the chain handler method or lambda method. Use this overload if you need async but no cancellation token
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="handlerFunc">The method to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageBusSubscription Handler<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, Task> handlerFunc)
        {
            return messageHandlerChainBuilder.Handler((message, token) => handlerFunc(message));
        }


        /// <summary>
        /// Set the chain handler method to a ISimpleMessageHandler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="messageHandler">The ISimpleMessageHandler to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageBusSubscription Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            ISimpleMessageHandler<TMessageType> messageHandler)
        {
            return messageHandlerChainBuilder.Handler((message, token) => messageHandler.HandleMessageAsync(message));
        }

        /// <summary>
        /// Set the chain message handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="messageHandler">The ISimpleMessageHandler to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageBusSubscription Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            IMessageHandler<TMessageType> messageHandler)
        {
            return messageHandlerChainBuilder.Handler(messageHandler.HandleMessageAsync);
        }

        /// <summary>
        /// Create a message handler chain to set up a subscription
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="subscriptions">The subscriptions interface</param>
        /// <returns>The new message handler chain</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Subscribe<TMessageType>(this IMessageBusSubscriptions<TMessageType> subscriptions)
        {
            return new MessageHandlerChainBuilder<TMessageType>(subscriptions);
        }
    }
}