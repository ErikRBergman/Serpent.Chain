// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.Helpers;
    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.Models;

    public static class ParallelMessageHandlerChainPublisherExtensions
    {
        public static ConcurrentMessageBusOptions<TMessageType> Parallel<TMessageType>(this IMessageHandlerDispatchOptions<TMessageType> messageHandlerDispatchOptions)
        {
            messageHandlerDispatchOptions.Options.BusPublisher = ParallelPublisher<TMessageType>.Default;
            return messageHandlerDispatchOptions.Options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> Parallel<TMessageType>(
            this IMessageHandlerDispatchOptions<TMessageType> messageHandlerDispatchOptions,
            Action<MessageHandlerChainBuilder<MessageAndSubscription<TMessageType>>> messageHandlerChainBuilderAction)
        {
            var builder = new MessageHandlerChainBuilder<MessageAndSubscription<TMessageType>>(NullMessageSubscriber<MessageAndSubscription<TMessageType>>.Default);

            messageHandlerChainBuilderAction(builder);

            if (builder.Count == 0)
            {
                return messageHandlerDispatchOptions.Parallel();
            }

            messageHandlerDispatchOptions.Options.BusPublisher = new ParallelMessageHandlerChainPublisher<TMessageType>(builder);

            return messageHandlerDispatchOptions.Options;
        }

        public static IMessageHandlerDispatchOptions<TMessageType> Dispatch<TMessageType>(this ConcurrentMessageBusOptions<TMessageType> options)
        {
            return new MessageHandlerDispatchOptions<TMessageType>(options);
        }
    }
}