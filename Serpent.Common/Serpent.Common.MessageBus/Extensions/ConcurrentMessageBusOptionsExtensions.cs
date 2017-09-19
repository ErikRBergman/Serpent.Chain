// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class ConcurrentMessageBusOptionsExtensions
    {
        public static ConcurrentMessageBusOptions<TMessageType> DisableWeakReferenceGarbageCollection<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            var garbageCollectionOptions = new ConcurrentMessageBusOptions<TMessageType>.WeakReferenceGarbageCollectionOptions
            {
                IsEnabled = false
            };

            options.WeakReferenceGarbageCollection = garbageCollectionOptions;
            return options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> EnableWeakReferenceGarbageCollection<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            TimeSpan? interval = null)
        {
            var garbageCollectionOptions = new ConcurrentMessageBusOptions<TMessageType>.WeakReferenceGarbageCollectionOptions
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

        public static ConcurrentMessageBusOptions<TMessageType> UseBackgroundSemaphorePublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            int concurrencyLevel = -1,
            BusPublisher<TMessageType> innerPublisher = null)
        {
            options.UseCustomPublisher(new BackgroundSemaphorePublisher<TMessageType>(concurrencyLevel, innerPublisher));
            return options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseCustomPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            BusPublisher<TMessageType> customBusPublisher)
        {
            options.BusPublisher = customBusPublisher;
            return options;
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseFireAndForgetPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            BusPublisher<TMessageType> innerPublisher)
        {
            return options.UseCustomPublisher(new FireAndForgetPublisher<TMessageType>(innerPublisher));
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseFireAndForgetPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options)
        {
            return options.UseCustomPublisher(FireAndForgetPublisher<TMessageType>.Default);
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseLoggingPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            BusPublisher<TMessageType> busPublisher,
            Action<TMessageType> logAction)
        {
            return options.UseCustomPublisher(new LoggingPublisher<TMessageType>(busPublisher, logAction));
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseLoggingPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            BusPublisher<TMessageType> busPublisher,
            Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, BusPublisher<TMessageType>, Task> publishFunc)
        {
            return options.UseCustomPublisher(new LoggingPublisher<TMessageType>(busPublisher, publishFunc));
        }

        public static ConcurrentMessageBusOptions<TMessageType> UseLoggingPublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            BusPublisher<TMessageType> busPublisher,
            Func<TMessageType, BusPublisher<TMessageType>, Task> publishFunc)
        {
            return options.UseCustomPublisher(new LoggingPublisher<TMessageType>(busPublisher, publishFunc));
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

        public static ConcurrentMessageBusOptions<TMessageType> UseSemaphorePublisher<TMessageType>(
            this ConcurrentMessageBusOptions<TMessageType> options,
            int concurrencyLevel = -1,
            BusPublisher<TMessageType> innerPublisher = null)
        {
            return options.UseCustomPublisher(new SemaphorePublisher<TMessageType>(concurrencyLevel, innerPublisher));
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
            Func<ISubscription<TMessageType>, TMessageType, Task> customHandlerMethod = null)
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