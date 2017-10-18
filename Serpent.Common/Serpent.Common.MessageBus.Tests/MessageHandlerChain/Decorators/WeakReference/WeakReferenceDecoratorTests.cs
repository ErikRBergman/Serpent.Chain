using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serpent.Common.MessageBus.Tests.MessageHandlerChain.Decorators.WeakReference
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    [TestClass]
    public class WeakReferenceDecoratorTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            //new int[] { 1 }.SelectMany()

            var bus = new ConcurrentMessageBus<int>();

            Assert.AreEqual(0, bus.SubscriberCount);

            // Strong references
            bus.Subscribe()
                .Concurrent(5)
                .Retry(2, TimeSpan.FromSeconds(10))
                    .Handler(new MyHandler());

            Assert.AreEqual(1, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);

            await bus.PublishAsync();

            Assert.AreEqual(1, bus.SubscriberCount);

            // Now weak reference
            bus.Subscribe().WeakReference().Handler(new MyHandler());

            Assert.AreEqual(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.AreEqual(1, bus.SubscriberCount);

            // Now weak reference with more decorators
            bus.Subscribe().WeakReference().Delay(50).Concurrent(10).Handler(new MyHandler());

            Assert.AreEqual(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.AreEqual(1, bus.SubscriberCount);


            // Weak reference with more decorators first
            bus.Subscribe().Delay(50).Concurrent(10).WeakReference().Delay(50).Concurrent(10).Handler(new MyHandler());

            Assert.AreEqual(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.AreEqual(1, bus.SubscriberCount);



        }
    }

    internal class MyHandler : IMessageHandler<int>
    {
        public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
