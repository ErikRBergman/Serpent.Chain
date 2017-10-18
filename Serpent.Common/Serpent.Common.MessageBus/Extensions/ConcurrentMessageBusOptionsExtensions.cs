// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public static class ConcurrentMessageBusOptionsExtensions
    {
        public static ConcurrentMessageBusOptions<TMessageType> UseCustomPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            BusPublisher<TMessageType> customBusPublisher)
        {
            options.BusPublisher = customBusPublisher;
            return options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseForcedParallelPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            options.BusPublisher = ForcedParallelPublisher<TMessageType>.Default;
            return options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseParallelPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            options.BusPublisher = ParallelPublisher<TMessageType>.Default;
            return options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseFuncPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            Func<IEnumerable<Func<TMessageType, CancellationToken, Task>>, TMessageType, Task> publishFunc)
        {
            return options.UseCustomPublisher(new FuncPublisher<TMessageType>(publishFunc));
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseSerialPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            return options.UseCustomPublisher(SerialPublisher<TMessageType>.Default);
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseSingleReceiverPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            Func<Func<TMessageType, CancellationToken, Task>, TMessageType, CancellationToken, Task> customHandlerMethod = null)
        {
            return options.UseCustomPublisher(new SingleReceiverPublisher<TMessageType>(customHandlerMethod));
        }
    }
}