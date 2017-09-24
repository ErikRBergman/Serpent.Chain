namespace Serpent.Common.MessageBus.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConcurrentMessageBusOptionsExtensionsTests
    {
        [TestMethod]
        public void DisableWeakReferenceGarbageCollectionTests()
        {
            var options = new ConcurrentMessageBusOptions<int>
            {
                WeakReferenceGarbageCollection = new WeakReferenceGarbageCollectionOptions()
                {
                    IsEnabled = true
                }
            };

            Assert.AreEqual(true, options.WeakReferenceGarbageCollection.IsEnabled);

            options.DisableWeakReferenceGarbageCollection();

            Assert.AreEqual(false, options.WeakReferenceGarbageCollection.IsEnabled);
        }

        [TestMethod]
        public void EnableWeakReferenceGarbageCollectionTests()
        {
            // Ensure collection is enabled and collection interval is preserved when omitting collection interval
            var options = new ConcurrentMessageBusOptions<int>
            {
                WeakReferenceGarbageCollection = new WeakReferenceGarbageCollectionOptions()
                {
                    IsEnabled = false,
                    CollectionInterval = TimeSpan.FromHours(20)
                }
            };

            Assert.AreEqual(false, options.WeakReferenceGarbageCollection.IsEnabled);

            options.EnableWeakReferenceGarbageCollection();

            Assert.AreEqual(true, options.WeakReferenceGarbageCollection.IsEnabled);
            Assert.AreEqual(TimeSpan.FromHours(20), options.WeakReferenceGarbageCollection.CollectionInterval);

            // Ensure collection is enabled and collection interval overwritten 
            options = new ConcurrentMessageBusOptions<int>
            {
                WeakReferenceGarbageCollection = new WeakReferenceGarbageCollectionOptions()
                {
                    IsEnabled = false,
                    CollectionInterval = TimeSpan.FromHours(20)
                }
            };

            Assert.AreEqual(false, options.WeakReferenceGarbageCollection.IsEnabled);

            options.EnableWeakReferenceGarbageCollection(TimeSpan.FromSeconds(20));

            Assert.AreEqual(true, options.WeakReferenceGarbageCollection.IsEnabled);
            Assert.AreEqual(TimeSpan.FromSeconds(20), options.WeakReferenceGarbageCollection.CollectionInterval);
        }

        [TestMethod]
        public void UseBackgroundSemaphorePublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseBackgroundSemaphorePublisher();
            Assert.AreEqual(typeof(BackgroundSemaphorePublisher<int>), options.BusPublisher.GetType());
        }

        [TestMethod]
        public void UseCustomPublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseCustomPublisher(SerialPublisher<int>.Default);
            Assert.AreSame(SerialPublisher<int>.Default, options.BusPublisher);
        }

        [TestMethod]
        public async Task UseFireAndForgetPublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseFireAndForgetPublisher();
            Assert.AreEqual(typeof(FireAndForgetPublisher<int>), options.BusPublisher.GetType());
            Assert.AreSame(FireAndForgetPublisher<int>.Default, options.BusPublisher);

            var bag = new ConcurrentBag<int>();

            options.UseFireAndForgetPublisher(
                new FuncPublisher<int>(
                    (subscriptions, message) =>
                        {
                            bag.Add(message);
                            return Task.CompletedTask;
                        }));

            Assert.AreEqual(typeof(FireAndForgetPublisher<int>), options.BusPublisher.GetType());
            Assert.AreNotSame(FireAndForgetPublisher<int>.Default, options.BusPublisher);

            var bus = new ConcurrentMessageBus<int>(options);

            await bus.PublishAsync(1);
            await bus.PublishAsync(2);
            await bus.PublishAsync(3);

            await Task.Delay(50);

            Assert.IsTrue(bag.Any(m => m == 1));
            Assert.IsTrue(bag.Any(m => m == 2));
            Assert.IsTrue(bag.Any(m => m == 3));
        }

        [TestMethod]
        public void UseForcedParallelPublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseForcedParallelPublisher();
            Assert.AreEqual(typeof(ForcedParallelPublisher<int>), options.BusPublisher.GetType());
        }

        [TestMethod]
        public void UseParallelPublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseParallelPublisher();
            Assert.AreEqual(typeof(ParallelPublisher<int>), options.BusPublisher.GetType());
        }

        [TestMethod]
        public void UseSemaphorePublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseSemaphorePublisher();
            Assert.AreEqual(typeof(SemaphorePublisher<int>), options.BusPublisher.GetType());
        }

        [TestMethod]
        public void UseSerialPublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseSerialPublisher();
            Assert.AreEqual(typeof(SerialPublisher<int>), options.BusPublisher.GetType());
        }

        [TestMethod]
        public void UseSingleReceiverPublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseSingleReceiverPublisher();
            Assert.AreEqual(typeof(SingleReceiverPublisher<int>), options.BusPublisher.GetType());
        }

        [TestMethod]
        public void UseStrongReferencesTests()
        {
            var options = new ConcurrentMessageBusOptions<int>
            {
                SubscriptionReferenceType = SubscriptionReferenceTypeType.WeakReferences
            };

            Assert.AreEqual(SubscriptionReferenceTypeType.WeakReferences, options.SubscriptionReferenceType);
            options.UseStrongReferences();
            Assert.AreEqual(SubscriptionReferenceTypeType.StrongReferences, options.SubscriptionReferenceType);
        }

        [TestMethod]
        public void UseWeakReferencesTests()
        {
            var options = new ConcurrentMessageBusOptions<int>
            {
                SubscriptionReferenceType = SubscriptionReferenceTypeType.StrongReferences
            };

            Assert.AreEqual(SubscriptionReferenceTypeType.StrongReferences, options.SubscriptionReferenceType);
            options.UseWeakReferences();
            Assert.AreEqual(SubscriptionReferenceTypeType.WeakReferences, options.SubscriptionReferenceType);
        }
    }
}