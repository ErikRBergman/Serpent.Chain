namespace Serpent.Common.MessageBus.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public static void Publish<TMessageType>(this IServiceProvider serviceProvider, TMessageType message)
        {
            serviceProvider.Publisher<TMessageType>().Publish(message);
        }

        public static IMessageBusPublisher<TMessageType> Publisher<TMessageType>(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IMessageBusPublisher<TMessageType>>();
        }

        public static void PublishRange<TMessageType>(this IServiceProvider serviceProvider, IEnumerable<TMessageType> messages)
        {
            serviceProvider.Publisher<TMessageType>().PublishRange(messages);
        }

        public static IMessageBusSubscriptions<TMessageType> Subscriptions<TMessageType>(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IMessageBusSubscriptions<TMessageType>>();
        }

        /// <summary>
        /// Subscribe to a message using a message handler chain builder
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="serviceProvider">
        /// The service provider
        /// </param>
        /// <returns>
        /// The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;"/> used to setup the message handler chain.
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> Subscribe<TMessageType>(this IServiceProvider serviceProvider)
        {
            return serviceProvider
                .Subscriptions<TMessageType>()
                    .Subscribe();
        }

    }
}