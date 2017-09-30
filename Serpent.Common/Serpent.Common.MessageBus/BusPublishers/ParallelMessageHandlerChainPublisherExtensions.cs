// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.BusPublishers;
    using Serpent.Common.MessageBus.Exceptions;
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

        public static ConcurrentMessageBusOptions<TMessageType> Dispatch<TMessageType>(
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

        public static ConcurrentMessageBusOptions<TMessageType> Dispatch<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            Action<MessageHandlerChainBuilder<MessageAndSubscription<TMessageType>>, Func<MessageAndSubscription<TMessageType>, Task>> setupMessageHandlerChainAction)
        {
            var dispatch = new MessageHandlerPublishDispatch<MessageAndSubscription<TMessageType>>();

            var builder = new MessageHandlerChainBuilder<MessageAndSubscription<TMessageType>>(dispatch);
            setupMessageHandlerChainAction(builder, PublishToSubscription.PublishAsync<TMessageType>);

            if (dispatch.InvocationFunc == null)
            {
                throw new NoHandlerException("No handler was added to the message handler chain. Messages can not be dispatched to the bus.\r\nUse .Handler() or .Factory() on the message handler chain.");
            }

            options.BusPublisher = new ParallelMessageHandlerChainPublisher<TMessageType>(dispatch.InvocationFunc);
            return options;
        }

        //public static IMessageHandlerDispatchOptions<TMessageType> Dispatch<TMessageType>(this ConcurrentMessageBusOptions<TMessageType> options)
        //{
        //    return new MessageHandlerDispatchOptions<TMessageType>(options);
        //}
    }
}