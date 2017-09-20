// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class SubscriptionBuilderExtensions
    {
        public static IMessageBusSubscription Handler<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, Action<TMessageType> handlerFunc)
        {
            return subscriptionBuilder.Handler(
                message =>
                    {
                        handlerFunc(message);
                        return Task.CompletedTask;
                    });
        }

        public static SubscriptionBuilder<TMessageType> Add<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, Func<Func<TMessageType, Task>, BusSubscription<TMessageType>> addFunc)
        {
            return subscriptionBuilder.Add(previousHandler => addFunc(previousHandler).HandleMessageAsync);
        }

        public static SubscriptionBuilder<TMessageType> Subscribe<TMessageType>(this IMessageBusSubscriber<TMessageType> subscriber)
        {
            return new SubscriptionBuilder<TMessageType>(subscriber);
        }
    }
}