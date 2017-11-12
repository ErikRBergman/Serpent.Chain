// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Helpers;
    using Serpent.MessageBus.Interfaces;
    using Serpent.MessageBus.MessageHandlerChain;
    using Serpent.MessageBus.MessageHandlerChain.Decorators;

    /// <summary>
    ///     Extensions for IMessageHandlerChainBuilder
    /// </summary>
    public static class MessageHandlerChainBuilderExtensions
    {
        /// <summary>
        /// Builds the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        /// The message Handler Chain Builder.
        /// </param>
        /// <returns>
        /// The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;"/>.
        /// </returns>
        public static Func<TMessageType, CancellationToken, Task> BuildFunc<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            var subscriptionNotification = new MessageHandlerChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);
            var func = messageHandlerChainBuilder.BuildFunc(services);

            var chain = new MessageHandlerChain<TMessageType>(func, ActionHelpers.NoAction);
            subscriptionNotification.Notify(chain);

            return func;
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="addFunc">The method that returns the </param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainDecorator<TMessageType>> addFunc)
        {
            return messageHandlerChainBuilder.AddDecorator(previousHandler => addFunc(previousHandler).HandleMessageAsync);
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="decoratorBuilder">The decorator builder</param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            IDecoratorBuilder<TMessageType> decoratorBuilder)
        {
            return messageHandlerChainBuilder.AddDecorator(decoratorBuilder.BuildDecorator());
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="addFunc">The method that returns the </param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, MessageHandlerChainDecorator<TMessageType>> addFunc)
        {
            return messageHandlerChainBuilder.Decorate((previousHandler, services) => addFunc(previousHandler, services).HandleMessageAsync);
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="addFunc">The method that returns the </param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> addFunc)
        {
            return messageHandlerChainBuilder.Decorate((previousHandler, services) => addFunc(previousHandler));
        }

        /// <summary>
        ///     Use a factory method to provide a message handler instance for each message passing through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="handlerFactory">The handler factory method</param>
        /// <param name="neverDispose">Prevent the subscription from disposing the message handler</param>
        public static void AddHandlerFactory<TMessageType, THandler>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<THandler> handlerFactory,
            bool neverDispose = false)
            where THandler : IMessageHandler<TMessageType>
        {
            if (neverDispose == false && typeof(IDisposable).IsAssignableFrom(typeof(THandler)))
            {
                messageHandlerChainBuilder.Handler(
                    async (message, token) =>
                        {
                            var handler = handlerFactory();
                            try
                            {
                                await handler.HandleMessageAsync(message, token);
                            }
                            finally
                            {
                                ((IDisposable)handler).Dispose();
                            }
                        });
            }

            messageHandlerChainBuilder.Handler(
                (message, token) =>
                    {
                        var handler = handlerFactory();
                        return handler.HandleMessageAsync(message, token);
                    });
        }

        /// <summary>
        ///     Set the chain handler method or lambda method. Use this overload if your handler has no need for async or
        ///     cancellationToken
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="handlerFunc">The method to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            return messageHandlerChainBuilder.Handle(services => handlerFunc);
        }

        /// <summary>
        ///     Set the chain handler method or lambda method. Use this overload if your handler has no need for async or
        ///     cancellationToken
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="handlerAction">The action to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<TMessageType> handlerAction)
        {
            return messageHandlerChainBuilder.Handler(
                (message, token) =>
                    {
                        handlerAction(message);
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        ///     Set the chain handler method or lambda method. Use this overload if you need async but no cancellation token
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="handlerFunc">The method to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task> handlerFunc)
        {
            return messageHandlerChainBuilder.Handler((message, token) => handlerFunc(message));
        }

        /// <summary>
        ///     Set the chain handler method to a ISimpleMessageHandler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="messageHandler">The ISimpleMessageHandler to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            ISimpleMessageHandler<TMessageType> messageHandler)
        {
            return messageHandlerChainBuilder.Handler((message, token) => messageHandler.HandleMessageAsync(message));
        }

        /// <summary>
        ///     Set the chain message handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mhc builder</param>
        /// <param name="messageHandler">The ISimpleMessageHandler to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Handler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            IMessageHandler<TMessageType> messageHandler)
        {
            return messageHandlerChainBuilder.Handler(messageHandler.HandleMessageAsync);
        }

        /// <summary>
        ///     Create a message handler chain to set up a subscription
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="subscriptions">The subscriptions interface</param>
        /// <param name="setupAction">The method called to configure the message handler chain for the new subscription</param>
        /// <returns>The new message handler chain</returns>
        public static IMessageHandlerChain<TMessageType> Subscribe<TMessageType>(
            this IMessageBusSubscriptions<TMessageType> subscriptions,
            Action<IMessageHandlerChainBuilder<TMessageType>> setupAction)
        {
            var builder = new MessageHandlerChainBuilder<TMessageType>();
            setupAction(builder);

            var subscriptionNotification = new MessageHandlerChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);
            var chainFunc = builder.BuildFunc(services);

            var subscription = subscriptions.Subscribe(chainFunc);

            var chain = new MessageHandlerChain<TMessageType>(chainFunc, subscription.Dispose);
            subscriptionNotification.Notify(chain);

            return chain;
        }
    }
}