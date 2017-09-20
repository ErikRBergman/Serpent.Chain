// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class RetrySubscriptionExtensions
    {
        public static IMessageBusSubscription CreateRetrySubscription<TMessageType>(
            this IMessageBusSubscriber<TMessageType> messageBus,
            Func<TMessageType, Task> handlerFunc,
            int maxNumberOfAttempts,
            TimeSpan retryDelay)
        {
            return messageBus.Subscribe(new RetrySubscription<TMessageType>(handlerFunc, maxNumberOfAttempts, retryDelay).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateRetrySubscription<TMessageType>(
            this IMessageBusSubscriber<TMessageType> messageBus,
            IMessageHandler<TMessageType> handler,
            int maxNumberOfAttempts,
            TimeSpan retryDelay)
        {
            return messageBus.Subscribe(new RetrySubscription<TMessageType>(handler.HandleMessageAsync, maxNumberOfAttempts, retryDelay).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateRetrySubscription<TMessageType>(
            this IMessageBusSubscriber<TMessageType> messageBus,
            BusSubscription<TMessageType> innerSubscription,
            int maxNumberOfAttempts,
            TimeSpan retryDelay)
        {
            return messageBus.Subscribe(new RetrySubscription<TMessageType>(innerSubscription, maxNumberOfAttempts, retryDelay).HandleMessageAsync);
        }

        /// <summary>
        /// Retry the message dispatch
        /// </summary>
        /// <typeparam name="TMessageType">Message type</typeparam>
        /// <param name="subscriptionBuilder">The subscription builder.</param>
        /// <param name="maxNumberOfAttempts">The maximum number of attempts to try.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <param name="exceptionFunc">Optional function called for each time the message handler throws an exception.</param>
        /// <param name="successFunc">Option function called after the messgae handler has succeeded.</param>
        /// <returns></returns>
        public static SubscriptionBuilder<TMessageType> Retry<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            int maxNumberOfAttempts,
            TimeSpan retryDelay,
            Func<TMessageType, int, Exception, Task> exceptionFunc = null, 
            Func<TMessageType, Task> successFunc = null)
        {
            return subscriptionBuilder.Add(currentHandler => new RetrySubscription<TMessageType>(currentHandler, maxNumberOfAttempts, retryDelay, exceptionFunc, successFunc).HandleMessageAsync);
        }
    }
}