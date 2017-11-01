namespace Serpent.Common.MessageBus.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConcurrentMessageBusOptionsExtensionsTests
    {
        [TestMethod]
        public async Task UseCustomPublisherTests()
        {
            var mypublisher = new MyPublisher();
            var bus = new ConcurrentMessageBus<int>(options => options.UseCustomPublisher(mypublisher));

            await bus.PublishAsync(55);
            await bus.PublishAsync(1);
            await bus.PublishAsync(5);

            Assert.AreEqual(3, mypublisher.Messages.Count);

            Assert.AreEqual(55, mypublisher.Messages[0]);
            Assert.AreEqual(1, mypublisher.Messages[1]);
            Assert.AreEqual(5, mypublisher.Messages[2]);
        }

        [TestMethod]
        public async Task UseSubscriptionChainWithHandlerTests()
        {
            // Test having message handler decorators both in the publish dispatch and the Subscription
            var bus = new ConcurrentMessageBus<TestMessage>(
                options => options.UseSubscriptionChain(
                    (chain, handler) =>
                        {
                            chain.Filter(message => message.Message.Log.TryAdd("Before", DateTime.Now), message => message.Message.Log.TryAdd("After", DateTime.Now))
                                .Handler(handler);
                        }));

            bus.Subscribe(builder => builder
                .Handler(
                    async message =>
                        {
                            await Task.Delay(100);
                            message.Log.TryAdd("Handler", DateTime.Now);
                            await Task.Delay(100);
                        }));

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
        public async Task UseSubscriptionChainWithoutHandlerTests()
        {
            // Test having message handler decorators both in the publisher and the subscription
            var bus = new ConcurrentMessageBus<TestMessage>(
                options => options.UseSubscriptionChain(
                    chain => chain
                        .Filter(
                            message => message.Message.Log.TryAdd("Before", DateTime.Now), 
                            message => message.Message.Log.TryAdd("After", DateTime.Now))));

            bus.Subscribe(builder =>
                builder.Handler(
                    async message =>
                        {
                            await Task.Delay(100);
                            message.Log.TryAdd("Handler", DateTime.Now);
                            await Task.Delay(100);
                        }));

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

        private class MyPublisher : BusPublisher<int>
        {
            public List<int> Messages { get; } = new List<int>();

            public override Task PublishAsync(IEnumerable<Func<int, CancellationToken, Task>> handlers, int message, CancellationToken cancellationToken)
            {
                this.Messages.Add(message);
                return Task.CompletedTask;
            }
        }

        private class TestMessage
        {
            public ConcurrentDictionary<string, DateTime> Log { get; } = new ConcurrentDictionary<string, DateTime>();
        }
    }
}