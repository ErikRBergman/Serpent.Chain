namespace Serpent.Common.MessageBus.Tests
{
    using System;
    using System.Collections.Concurrent;
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
                    CollectionInterval =
                                                                               TimeSpan.FromHours(
                                                                                   20)
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
                    CollectionInterval =
                                                                           TimeSpan.FromHours(20)
                }
            };

            Assert.AreEqual(false, options.WeakReferenceGarbageCollection.IsEnabled);

            options.EnableWeakReferenceGarbageCollection(TimeSpan.FromSeconds(20));

            Assert.AreEqual(true, options.WeakReferenceGarbageCollection.IsEnabled);
            Assert.AreEqual(TimeSpan.FromSeconds(20), options.WeakReferenceGarbageCollection.CollectionInterval);
        }

        [TestMethod]
        public async Task PublisherTests()
        {
            // Test having message handler decorators both in the publish dispatch and the Subscription
            var bus = new ConcurrentMessageBus<TestMessage>(
                options => options.Dispatch(
                        (chain, handler) =>
                            {
                                chain
                                    .Filter(message => { message.Message.Log.TryAdd("Before", DateTime.Now); }, message => { message.Message.Log.TryAdd("After", DateTime.Now); })
                                    .Handler(handler);
                            }));

            bus.Subscribe()
                .Handler(
                    async message =>
                        {
                            await Task.Delay(100);
                            message.Log.TryAdd("Handler", DateTime.Now);
                            await Task.Delay(100);
                        });

            var msg = new TestMessage();
            bus.Publish(msg);

            await Task.Delay(50);

            Assert.AreEqual(1, msg.Log.Count);
            Assert.IsTrue(msg.Log.ContainsKey("Before"));

            await Task.Delay(100);

            Assert.AreEqual(2, msg.Log.Count);
            Assert.IsTrue(msg.Log.ContainsKey("Before"));
            Assert.IsTrue(msg.Log.ContainsKey("Handler"));

            await Task.Delay(100);

            Assert.AreEqual(3, msg.Log.Count);
            Assert.IsTrue(msg.Log.ContainsKey("Before"));
            Assert.IsTrue(msg.Log.ContainsKey("Handler"));
            Assert.IsTrue(msg.Log.ContainsKey("After"));
        }

        [TestMethod]
        public void UseCustomPublisherTests()
        {
            var options = new ConcurrentMessageBusOptions<int>();
            options.UseCustomPublisher(SerialPublisher<int>.Default);
            Assert.AreSame(SerialPublisher<int>.Default, options.BusPublisher);
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

        private class TestMessage
        {
            public ConcurrentDictionary<string, DateTime> Log { get; set; } = new ConcurrentDictionary<string, DateTime>();
        }
    }
}