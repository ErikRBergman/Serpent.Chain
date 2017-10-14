// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ConcurrentMessageBusOptionsExtensions
    {
        /// <summary>
        /// Disables the weak reference garbage collection process
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="options">The bus options</param>
        /// <returns>Options</returns>
        public static ConcurrentMessageBusOptions<TMessageType> DisableWeakReferenceGarbageCollection<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            var garbageCollectionOptions = new WeakReferenceGarbageCollectionOptions
            {
                IsEnabled = false
            };

            options.WeakReferenceGarbageCollection = garbageCollectionOptions;
            return options;
        }

        /// <summary>
        /// Disables the weak reference garbage collection process
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="options">
        /// The bus options
        /// </param>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <returns>
        /// Options
        /// </returns>
         public static ConcurrentMessageBusOptions<TMessageType> EnableWeakReferenceGarbageCollection<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            TimeSpan? interval = null)
        {
            var garbageCollectionOptions = new WeakReferenceGarbageCollectionOptions
            {
                IsEnabled = true,
                CollectionInterval = options.WeakReferenceGarbageCollection.CollectionInterval
            };

            if (interval != null)
            {
                garbageCollectionOptions.CollectionInterval = interval.Value;
            }

            options.WeakReferenceGarbageCollection = garbageCollectionOptions;
            return options;
        }

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
            Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, Task> publishFunc)
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
            Func<ISubscription<TMessageType>, TMessageType, CancellationToken, Task> customHandlerMethod = null)
        {
            return options.UseCustomPublisher(new SingleReceiverPublisher<TMessageType>(customHandlerMethod));
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseStrongReferences<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            options.SubscriptionReferenceType = SubscriptionReferenceTypeType.StrongReferences;
            return options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseWeakReferences<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            options.SubscriptionReferenceType = SubscriptionReferenceTypeType.WeakReferences;
            return options;
        }
    }
}