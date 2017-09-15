// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class FireAndForgetSubscriptionMessageBusSubscriberExtensions
    {
        public static SubscriptionWrapper<T> CreateFireAndForgetSubscription<T>(this IMessageBusSubscriber<T> messageBus, Func<T, Task> handlerFunc)
        {
            var subscription = messageBus.Subscribe(new FireAndForgetSubscription<T>(handlerFunc).HandleMessageAsync);
            return new SubscriptionWrapper<T>(subscription);
        }

        public static SubscriptionWrapper<T> CreateFireAndForgetSubscription<T>(this IMessageBusSubscriber<T> messageBus, IMessageHandler<T> handler)
        {
            var subscription = messageBus.Subscribe(new FireAndForgetSubscription<T>(handler.HandleMessageAsync).HandleMessageAsync);
            return new SubscriptionWrapper<T>(subscription);
        }
    }
}