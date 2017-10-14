// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.BusPublishers;
    using Serpent.Common.MessageBus.Exceptions;
    using Serpent.Common.MessageBus.Models;

    /// <summary>
    /// Extensions for parallel message handler chain publisher
    /// </summary>
    public static class ParallelMessageHandlerChainPublisherExtensions
    {
        /// <summary>
        /// Sets up a message handler chain for the bus publisher
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="options">The options</param>
        /// <param name="setupMessageHandlerChainAction">The action called to setup the message handler chain</param>
        /// <returns>Bus options</returns>
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

            options.BusPublisher = new ParallelMessageHandlerChainPublisher<TMessageType>(builder.Build(dispatch.InvocationFunc));
            return options;
        }
    }
}