namespace Serpent.Common.MessageBus.Tests.SubscriptionTypes
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;
    using System.Linq;

    [TestClass]
    public class LimitedThroughputSubscriptionTests
    {
        [TestMethod]
        public async Task TestLimitedThroughputSubscription()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            int count = 0;

            var times = new ConcurrentDictionary<TimeSpan, int>();

            var sub = bus.Subscribe()
                .LimitedThroughput(10, TimeSpan.FromMilliseconds(100))
                .Handler(msgz =>
                    {
                        Interlocked.Increment(ref count);
                    });

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < 2000; i++)
            {
                await bus.PublishAsync();
            }

            await Task.Delay(150);

            Assert.AreEqual(20, count);

            await Task.Delay(450);
            Assert.AreEqual(60, count);

        }

        private class Message1
        {
            public string Status { get; set; }
        }

    }
}