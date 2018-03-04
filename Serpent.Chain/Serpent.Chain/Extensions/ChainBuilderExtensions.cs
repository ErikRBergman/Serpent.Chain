// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators;
    using Serpent.Chain.Helpers;
    using Serpent.Chain.Interfaces;
    using Serpent.Chain.Notification;

    /// <summary>
    ///     Extensions for IChainBuilder
    /// </summary>
    public static class ChainBuilderExtensions
    {
        /// <summary>
        /// Builds the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="chainBuilder">
        /// The message Handler chain Builder.
        /// </param>
        /// <returns>
        /// The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;"/>.
        /// </returns>
        public static Func<TMessageType, CancellationToken, Task> BuildFunc<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            var subscriptionNotification = new ChainBuilderNotifier();
            var services = new ChainBuilderSetupServices(subscriptionNotification);
            var func = chainBuilder.BuildFunc(services);

            var chain = new Chain<TMessageType>(func, ActionHelpers.NoAction);
            subscriptionNotification.Notify(chain);

            return func;
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="addFunc">The method that returns the </param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, ChainDecorator<TMessageType>> addFunc)
        {
            if (addFunc == null)
            {
                throw new ArgumentNullException(nameof(addFunc));
            }

            return chainBuilder.AddDecorator(previousHandler => addFunc(previousHandler).HandleMessageAsync);
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="decoratorBuilder">The decorator builder</param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            IDecoratorBuilder<TMessageType> decoratorBuilder)
        {
            if (decoratorBuilder == null)
            {
                throw new ArgumentNullException(nameof(decoratorBuilder));
            }

            return chainBuilder.AddDecorator(decoratorBuilder.BuildDecorator());
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="addFunc">The method that returns the </param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, ChainBuilderSetupServices, ChainDecorator<TMessageType>> addFunc)
        {
            if (addFunc == null)
            {
                throw new ArgumentNullException(nameof(addFunc));
            }

            return chainBuilder.Decorate((previousHandler, services) => addFunc(previousHandler, services).HandleMessageAsync);
        }

        /// <summary>
        ///     Add a decorator to the message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="addFunc">The method that returns the </param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> AddDecorator<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> addFunc)
        {
            if (addFunc == null)
            {
                throw new ArgumentNullException(nameof(addFunc));
            }

            return chainBuilder.Decorate((previousHandler, services) => addFunc(previousHandler));
        }

        /// <summary>
        ///     Use a factory method to provide a message handler instance for each message passing through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="handlerFactory">The handler factory method</param>
        /// <param name="neverDispose">Prevent the subscription from disposing the message handler</param>
        public static void AddHandlerFactory<TMessageType, THandler>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<THandler> handlerFactory,
            bool neverDispose = false)
            where THandler : IMessageHandler<TMessageType>
        {
            if (handlerFactory == null)
            {
                throw new ArgumentNullException(nameof(handlerFactory));
            }

            if (neverDispose == false && typeof(IDisposable).IsAssignableFrom(typeof(THandler)))
            {
                chainBuilder.Handler(
                    async (message, token) =>
                        {
                            var handler = handlerFactory();
                            try
                            {
                                await handler.HandleMessageAsync(message, token).ConfigureAwait(false);
                            }
                            finally
                            {
                                ((IDisposable)handler).Dispose();
                            }
                        });
            }

            chainBuilder.Handler(
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
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="handlerFunc">The method to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> Handler<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            if (handlerFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerFunc));
            }

            return chainBuilder.Handle(services => handlerFunc);
        }

        /// <summary>
        ///     Set the chain handler method or lambda method. Use this overload if your handler has no need for async or
        ///     cancellationToken
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="handlerAction">The action to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> Handler<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Action<TMessageType> handlerAction)
        {
            if (handlerAction == null)
            {
                throw new ArgumentNullException(nameof(handlerAction));
            }

            return chainBuilder.Handler(
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
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="handlerFunc">The method to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> Handler<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task> handlerFunc)
        {
            if (handlerFunc == null)
            {
                throw new ArgumentNullException(nameof(handlerFunc));
            }

            return chainBuilder.Handler((message, token) => handlerFunc(message));
        }

        /// <summary>
        ///     Set the chain handler method to a ISimpleMessageHandler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="messageHandler">The ISimpleMessageHandler to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> Handler<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            ISimpleMessageHandler<TMessageType> messageHandler)
        {
            return chainBuilder.Handler((message, token) => messageHandler.HandleMessageAsync(message));
        }

        /// <summary>
        ///     Set the chain message handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The mhc builder</param>
        /// <param name="messageHandler">The ISimpleMessageHandler to invoke</param>
        /// <returns>The mhc builder</returns>
        public static IChainBuilder<TMessageType> Handler<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            IMessageHandler<TMessageType> messageHandler)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            return chainBuilder.Handler(messageHandler.HandleMessageAsync);
        }
    }
}