// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class FireAndForgetSubscriptionMessageBusSubscriberExtensions
    {
        public static IMessageBusSubscription CreateFireAndForgetSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, Func<TMessageType, Task> handlerFunc)
        {
            return messageBus.Subscribe(new FireAndForgetSubscription<TMessageType>(handlerFunc).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateFireAndForgetSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, IMessageHandler<TMessageType> handler)
        {
            return messageBus.Subscribe(new FireAndForgetSubscription<TMessageType>(handler.HandleMessageAsync).HandleMessageAsync);
        }

        public static IMessageBusSubscription CreateFireAndForgetSubscription<TMessageType>(this IMessageBusSubscriber<TMessageType> messageBus, BusSubscription<TMessageType> innerSubscription)
        {
            return messageBus.Subscribe(new FireAndForgetSubscription<TMessageType>(innerSubscription).HandleMessageAsync);
        }
    }
}