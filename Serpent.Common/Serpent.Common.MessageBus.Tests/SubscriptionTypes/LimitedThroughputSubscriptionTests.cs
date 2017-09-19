namespace Serpent.Common.MessageBus.Tests.SubscriptionTypes
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LimitedThroughputSubscriptionTests
    {
        [TestMethod]
        public async Task TestLimitedThroughputSubscription()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            int count = 0;

            var sub = bus.CreateLimitedThroughputSubscription(
                msgz =>
                    {
                        Interlocked.Increment(ref count);
                        return Task.CompletedTask;
                    }, 
                10, 
                TimeSpan.FromMilliseconds(100));

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