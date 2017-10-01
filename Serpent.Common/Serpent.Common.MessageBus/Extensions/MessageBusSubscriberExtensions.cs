// ReSharper disable CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class MessageBusSubscriberExtensions
    {
        public static async Task<TMessageType> GetMessageAsync<TMessageType>(this IMessageBusSubscriptions<TMessageType> messageBus)
        {
            var completion = new TaskCompletionSource<TMessageType>();
            var isCompleted = 0;

            using (messageBus.Subscribe(
                    message =>
                        {
                            if (Interlocked.CompareExchange(ref isCompleted, 1, 0) == 0)
                            {
                                completion.SetResult(message);
                            }

                            return Task.CompletedTask;
                        })
                .Wrapper())
            {
                return await completion.Task.ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Register a subscription that through a factory creates a unique message handler for each message and then calls a
        ///     handler function
        ///     This functionality can also be replicated on your own by using:
        ///     messageBus.Subscribe(message => { var handler = new HandlerClass(); return handler.HandleMessageAsync(message); },
        ///     null);
        /// </summary>
        /// <typeparam name="TMessage">The bus message type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="messageBus">The message bus</param>
        /// <param name="messageHandlerFactoryFunc">The factory func (that creates the handler)</param>
        /// <param name="messageHandlerFactoryFuncSelector">The func that selectes the function to execute on the handler</param>
        /// <returns>A subscription wrapper</returns>
        public static SubscriptionWrapper RegisterFactory<TMessage, THandler>(
            this IMessageBusSubscriptions<TMessage> messageBus,
            Func<THandler> messageHandlerFactoryFunc,
            Func<THandler, Func<TMessage, CancellationToken, Task>> messageHandlerFactoryFuncSelector)
        {
            return SubscriptionWrapper.Create(
                messageBus,
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
        ///     messageBus.Subscribe(async message => { var handler = new HandlerClass(); await
        ///     handler.HandleMessageAsync(message); handler.Dispose(); }, null);
        /// </summary>
        /// <typeparam name="TMessage">The bus message type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="messageBus">The message bus</param>
        /// <param name="messageHandlerFactoryFunc">The factory func (that creates the handler)</param>
        /// <param name="messageHandlerFactoryFuncSelector">The func that selectes the function to execute on the handler</param>
        /// <returns>A subscription wrapper</returns>
        public static SubscriptionWrapper RegisterFactoryWithDisposableHandler<TMessage, THandler>(
            this IMessageBusSubscriptions<TMessage> messageBus,
            Func<THandler> messageHandlerFactoryFunc,
            Func<THandler, Func<TMessage, CancellationToken, Task>> messageHandlerFactoryFuncSelector)
            where THandler : IDisposable
        {
            return SubscriptionWrapper.Create(
                messageBus,
                async (message, token) =>
                    {
                        var handler = messageHandlerFactoryFunc();
                        var handlerFunc = messageHandlerFactoryFuncSelector(handler);
                        await handlerFunc(message, token).ConfigureAwait(false);
                        handler.Dispose();
                    });
        }

        /// <summary>
        ///     Subscribes to a messages, with an optional filter function
        /// </summary>
        /// <typeparam name="TMessageType">The message bus message type</typeparam>
        /// <param name="messageBus">the message bus</param>
        /// <param name="invocationFunc">The function to invoke when a message is received</param>
        /// <param name="messageFilterFunc">
        ///     A function filtering messages. If this function returns true, the messages is sent to
        ///     the invocation func
        /// </param>
        /// <returns>A subscription wrapper</returns>
        public static SubscriptionWrapper Subscribe<TMessageType>(
            this IMessageBusSubscriptions<TMessageType> messageBus,
            Func<TMessageType, CancellationToken, Task> invocationFunc,
            Func<TMessageType, bool> messageFilterFunc = null)
        {
            return SubscriptionWrapper.Create(messageBus, invocationFunc, messageFilterFunc);
        }

        /// <summary>
        ///     Subscribes to a defined type of message on a messages bus that handles multiple message types
        /// </summary>
        /// <typeparam name="TBaseMessage">The message bus message type</typeparam>
        /// <typeparam name="TMessage">The message type for this handlelr</typeparam>
        /// <param name="messageBus">the message bus</param>
        /// <param name="invocationFunc">The function to invoke when a message is received</param>
        /// <returns>A subscription wrapper</returns>
        public static SubscriptionWrapper Subscribe<TBaseMessage, TMessage>(
            this IMessageBusSubscriptions<TBaseMessage> messageBus,
            Func<TMessage, CancellationToken, Task> invocationFunc)
            where TMessage : TBaseMessage
        {
            return SubscriptionWrapper.Create(messageBus, (message, token) => invocationFunc((TMessage)message, token), message => message is TMessage);
        }

        /// <summary>
        ///     Subscribes to a defined type of message on a messages bus that handles multiple message types
        /// </summary>
        /// <typeparam name="TBaseMessage">The message bus message type</typeparam>
        /// <typeparam name="TMessage">The message type for this handlelr</typeparam>
        /// <param name="messageBus">the message bus</param>
        /// <param name="invocationFunc">The function to invoke when a message is received</param>
        /// <returns>A subscription wrapper</returns>
        public static SubscriptionWrapper Subscribe<TBaseMessage, TMessage>(this IMessageBusSubscriptions<TBaseMessage> messageBus, Func<TMessage, Task> invocationFunc)
            where TMessage : TBaseMessage
        {
            return SubscriptionWrapper.Create(messageBus, (message, token) => invocationFunc((TMessage)message), message => message is TMessage);
        }

        public static IMessageBusSubscription Subscribe<TMessageType>(this IMessageBusSubscriptions<TMessageType> subscriptions, Func<TMessageType, Task> handlerFunc)
        {
            return subscriptions.Subscribe((message, token) => handlerFunc(message));
        }

        /// <summary>
        ///     Supports subscribing to multiple types on a message bus with multiple message types
        /// </summary>
        /// <typeparam name="T">The base type of the message bus. For example, object or another chosen base class</typeparam>
        /// <param name="messageBus">The message bus</param>
        /// <param name="mapAction">A method that is called to setup the map</param>
        /// <returns>The new subscription</returns>
        public static IMessageBusSubscription Subscribe<T>(this IMessageBusSubscriptions<T> messageBus, Action<IMessageSubscriptionBuilder<T>> mapAction)
        {
            var builder = new MessageBusSubscriptionBuilder<T>();
            mapAction.Invoke(builder);
            var map = builder.InvocationMap;

            if (map.Count == 1)
            {
                var key = map.Keys.First();
                var func = map.Values.First();

                // Subscription to the same type as the message bus. 
                if (key == typeof(T))
                {
                    return messageBus.Subscribe<T>(func);
                }

                // Another type
                return messageBus.Subscribe(
                    (message, token) =>
                        {
                            if (message.GetType() == key)
                            {
                                return func(message, token);
                            }

                            return Task.CompletedTask;
                        });
            }

            return messageBus.Subscribe((message, token) => MapSubscriptionHandlerAsync(map, message, token));
        }

        private static Task MapSubscriptionHandlerAsync<T>(IReadOnlyDictionary<Type, Func<T, CancellationToken, Task>> map, T message, CancellationToken token)
        {
            if (map.TryGetValue(message.GetType(), out var handler))
            {
                return handler(message, token);
            }

            return Task.CompletedTask;
        }
    }
}