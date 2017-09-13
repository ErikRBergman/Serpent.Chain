namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class MessageBusSubscriberExtensions
    {
        public static SubscriptionWrapper<T> Subscribe<T>(this IMessageBusSubscriber<T> messageBus, Func<T, Task> invocationFunc, Func<T, bool> eventFilterFunc = null)
        {
            return SubscriptionWrapper<T>.Create(messageBus, invocationFunc, eventFilterFunc);
        }

        public static SubscriptionWrapper<TBusMessageType> Subscribe<TBusMessageType, TItemType>(this IMessageBusSubscriber<TBusMessageType> messageBus, Func<TItemType, Task> invocationFunc)
            where TItemType : TBusMessageType
        {
            return SubscriptionWrapper<TBusMessageType>.Create(messageBus, message => invocationFunc((TItemType)message), message => message is TItemType);
        }

        /// <summary>
        /// Supports subscribing to multiple types on a message bus with multiple message types
        /// </summary>
        /// <typeparam name="T">The base type of the message bus. For example, object or another chosen base class</typeparam>
        /// <param name="messageBus">The message bus</param>
        /// <param name="mapAction">A method that is called to setup the map</param>
        /// <returns>The new subscription</returns>
        public static IMessageBusSubscription Subscribe<T>(this IMessageBusSubscriber<T> messageBus, Action<IMessageSubscriptionBuilder<T>> mapAction)
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
                    return messageBus.Subscribe(func);
                }

                // Another type
                return messageBus.Subscribe(
                    message =>
                        {
                            if (message.GetType() == key)
                            {
                                return func(message);
                            }

                            return Task.CompletedTask;
                        });
            }

            return messageBus.Subscribe(message => MapSubscriptionHandlerAsync(map, message));
        }

        private static Task MapSubscriptionHandlerAsync<T>(IReadOnlyDictionary<Type, Func<T, Task>> map, T message)
        {
            if (map.TryGetValue(message.GetType(), out var handler))
            {
                return handler(message);
            }

            return Task.CompletedTask;
        }
    }
}