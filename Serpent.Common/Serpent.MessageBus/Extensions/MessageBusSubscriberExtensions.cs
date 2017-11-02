// ReSharper disable CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The message bus subscriber extension types
    /// </summary>
    public static class MessageBusSubscriberExtensions
    {
        /// <summary>
        ///     Registers a subscription that through a factory creates a unique message handler for each message and then calls a
        ///     handler function
        ///     This functionality can also be replicated on your own by using:
        ///     subscriptions.Subscribe(message => { var handler = new HandlerClass(); return handler.HandleMessageAsync(message); },
        ///     null);
        /// </summary>
        /// <typeparam name="TMessage">The bus message type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="subscriptions">The message bus</param>
        /// <param name="messageHandlerFactoryFunc">The factory func (that creates the handler)</param>
        /// <param name="messageHandlerFactoryFuncSelector">The func that selectes the function to execute on the handler</param>
        /// <returns>A subscription wrapper</returns>
        public static IMessageBusSubscription RegisterFactory<TMessage, THandler>(
            this IMessageBusSubscriptions<TMessage> subscriptions,
            Func<THandler> messageHandlerFactoryFunc,
            Func<THandler, Func<TMessage, CancellationToken, Task>> messageHandlerFactoryFuncSelector)
        {
            return subscriptions.Subscribe(
                (message, token) =>
                    {
                        var handler = messageHandlerFactoryFunc();
                        var handlerFunc = messageHandlerFactoryFuncSelector(handler);
                        return handlerFunc(message, token);
                    });
        }

        /// <summary>
        ///     Register a subscription that through a factory creates a unique message handler for each message, calls a handler
        ///     and then disposes the handler function
        ///     This functionality can also be replicated on your own by using:
        ///     subscriptions.Subscribe(async message => { var handler = new HandlerClass(); await
        ///     handler.HandleMessageAsync(message); handler.Dispose(); }, null);
        /// </summary>
        /// <typeparam name="TMessage">The bus message type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="subscriptions">The message bus</param>
        /// <param name="messageHandlerFactoryFunc">The factory func (that creates the handler)</param>
        /// <param name="messageHandlerFactoryFuncSelector">The func that selectes the function to execute on the handler</param>
        /// <returns>A subscription wrapper</returns>
        public static IMessageBusSubscription RegisterFactoryWithDisposableHandler<TMessage, THandler>(
            this IMessageBusSubscriptions<TMessage> subscriptions,
            Func<THandler> messageHandlerFactoryFunc,
            Func<THandler, Func<TMessage, CancellationToken, Task>> messageHandlerFactoryFuncSelector)
            where THandler : IDisposable
        {
            return subscriptions.Subscribe(
                async (message, token) =>
                    {
                        var handler = messageHandlerFactoryFunc();
                        var handlerFunc = messageHandlerFactoryFuncSelector(handler);
                        await handlerFunc(message, token).ConfigureAwait(false);
                        handler.Dispose();
                    });
        }

        /// <summary>
        /// Subscribes to a message bus
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="subscriptions">The subscriptions</param>
        /// <param name="handlerFunc">The handler method</param>
        /// <returns>A message bus subscription</returns>
        public static IMessageBusSubscription Subscribe<TMessageType>(this IMessageBusSubscriptions<TMessageType> subscriptions, Func<TMessageType, Task> handlerFunc)
        {
            return subscriptions.Subscribe((message, token) => handlerFunc(message));
        }
    }
}