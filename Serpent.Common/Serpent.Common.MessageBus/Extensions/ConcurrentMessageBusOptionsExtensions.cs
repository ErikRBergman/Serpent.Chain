// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Concurrent message bus options extensions
    /// </summary>
    public static class ConcurrentMessageBusOptionsExtensions
    {
        /// <summary>
        /// Use a custom bus publisher
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="options">The options</param>
        /// <param name="customBusPublisher">The custom bus publisher</param>
        /// <returns>The options</returns>
        public static ConcurrentMessageBusOptions<TMessageType> UseCustomPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            BusPublisher<TMessageType> customBusPublisher)
        {
            options.BusPublisher = customBusPublisher;
            return options;
        }

        /// <summary>
        /// The use the forced parallel publisher. Every message subscription handler is executed on a newly spawned Task
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="options">
        /// The options
        /// </param>
        /// <returns>
        /// The options
        /// </returns>
        public static ConcurrentMessageBusOptions<TMessageType> UseForcedParallelPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            options.BusPublisher = ForcedParallelPublisher<TMessageType>.Default;
            return options;
        }

        /// <summary>
        /// The use the parallel publisher.
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="options">
        /// The options
        /// </param>
        /// <returns>
        /// The options
        /// </returns>
        public static ConcurrentMessageBusOptions<TMessageType> UseParallelPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            options.BusPublisher = ParallelPublisher<TMessageType>.Default;
            return options;
        }

        /// <summary>
        /// The use a func publisher.
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="options">
        /// The options
        /// </param>
        /// <param name="publishFunc">
        /// The function called for each subscription
        /// </param>
        /// <returns>
        /// The options
        /// </returns>
        public static ConcurrentMessageBusOptions<TMessageType> UseFuncPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            Func<IEnumerable<Func<TMessageType, CancellationToken, Task>>, TMessageType, Task> publishFunc)
        {
            return options.UseCustomPublisher(new FuncPublisher<TMessageType>(publishFunc));
        }

        /// <summary>
        /// Use the serial publisher. Only a single subscription handler is executed at a time.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="options">The options</param>
        /// <returns>The options</returns>
        public static ConcurrentMessageBusOptions<TMessageType> UseSerialPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            return options.UseCustomPublisher(SerialPublisher<TMessageType>.Default);
        }

        /// <summary>
        /// Use a single receiver publisher
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="options">The options</param>
        /// <param name="customHandlerMethod">The custom </param>
        /// <returns></returns>
        public static ConcurrentMessageBusOptions<TMessageType> UseSingleReceiverPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            Func<Func<TMessageType, CancellationToken, Task>, TMessageType, CancellationToken, Task> customHandlerMethod = null)
        {
            return options.UseCustomPublisher(new SingleReceiverPublisher<TMessageType>(customHandlerMethod));
        }
    }
}