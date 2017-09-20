namespace Serpent.Common.MessageBus.Tests.SubscriptionTypes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SubscriptionBuilderTests
    {
        private class Message
        {
            public string Id { get; set; }

            public List<string> Steps { get; } = new List<string>();

            public string HandlerInvoked { get; set; }
        }

        [TestMethod]
        public async Task SubscriptionBuilderStackAllTests()
        {
            var bus = new ConcurrentMessageBus<Message>();

            int count = 0;

            var sub = bus
                .Subscribe()
                .NoDuplicates(message => message.Id)
                .FireAndForget()
                .Retry(5, TimeSpan.FromSeconds(5))
                .Semaphore(5)
                .LimitedThroughput(10, TimeSpan.FromSeconds(0.1))
                .Handler(
                    async message =>
                        {
                            Debug.WriteLine(DateTime.Now);
                            await Task.Delay(200);
                            message.HandlerInvoked = "Sure was";
                            Interlocked.Increment(ref count);
                        });

            for (int i = 0; i < 1000; i++)
            {
                await bus.PublishAsync();
            }

            await Task.Delay(300);

            Assert.AreEqual(1, count);

        }

        [TestMethod]
        public async Task SubscriptionBuilderStackTests()
        {
            var bus = new ConcurrentMessageBus<Message>();

            var sub = bus
                .Subscribe()
                .Filter(
                message =>
                    {
                        message.Steps.Add("before1");
                    },
                message =>
                    {
                        if (message.Steps.Contains("after2"))
                        {
                            message.Steps.Add("after1");
                        }
                    })
                .Filter(
                        message =>
                            {
                                if (message.Steps.Contains("before1"))
                                {
                                    message.Steps.Add("before2");
                                }

                                return true;
                            },
                        message =>
                            {
                                message.Steps.Add("after2");
                            })
                        .Handler(
                        message =>
                            {
                                message.Steps.Add("handler");
                                message.HandlerInvoked = "yes";
                            });

            var msg = new Message();
            await bus.PublishAsync(msg);

            Assert.AreEqual("yes", msg.HandlerInvoked);

            Assert.AreEqual("before1", msg.Steps[0]);
            Assert.AreEqual("before2", msg.Steps[1]);

            Assert.AreEqual("handler", msg.Steps[2]);

            Assert.AreEqual("after2", msg.Steps[3]);
            Assert.AreEqual("after1", msg.Steps[4]);
        }
    }
}