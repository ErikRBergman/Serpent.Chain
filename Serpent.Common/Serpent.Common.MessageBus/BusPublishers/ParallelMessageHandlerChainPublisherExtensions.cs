// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.BusPublishers;
    using Serpent.Common.MessageBus.Exceptions;
    using Serpent.Common.MessageBus.Models;

    public static class ParallelMessageHandlerChainPublisherExtensions
    {
        public static ConcurrentMessageBusOptions<TMessageType> Dispatch<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            Action<MessageHandlerChainBuilder<MessageAndSubscription<TMessageType>>, Func<MessageAndSubscription<TMessageType>, CancellationToken, Task>> setupMessageHandlerChainAction)
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
    }
}