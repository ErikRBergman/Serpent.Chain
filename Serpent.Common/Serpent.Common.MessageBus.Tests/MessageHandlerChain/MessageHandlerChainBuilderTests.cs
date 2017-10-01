namespace Serpent.Common.MessageBus.Tests.SubscriptionTypes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MessageHandlerChainBuilderTests
    {
        [TestMethod]
        public async Task MessageHandlerChainStackAllTest()
        {
            var bus = new ConcurrentMessageBus<Message>();

            var count = 0;

            using (bus.Subscribe()
                .FireAndForget()
                .SoftFireAndForget()
                .NoDuplicates(message => message.Id)
                .Concurrent(16)
                .ConcurrentFireAndForget(16)
                .Exception((msg, e) => Console.WriteLine(e))
                .Filter(null, null)
                .FireAndForget()
                .BranchOut(
                    branch => { branch.FireAndForget().Delay(TimeSpan.FromSeconds(10)).Handler(message => { Console.WriteLine("Sub branch 1"); }); },
                    branch => { branch.FireAndForget().Delay(TimeSpan.FromSeconds(20)).Handler(message => { Console.WriteLine("Sub branch 1"); }); })
                .Retry(5, TimeSpan.FromSeconds(5))
                .Semaphore(1)
                .LimitedThroughput(1, TimeSpan.FromSeconds(1))
                .Delay(TimeSpan.FromMilliseconds(50))
                .Select(message => new OuterMessage { Message = message, Token = CancellationToken.None })
                .Handler(
                    async message =>
                        {
                            Debug.WriteLine(DateTime.Now);
                            await Task.Delay(200);
                            message.Message.HandlerInvoked = "Sure was";
                            Interlocked.Increment(ref count);
                        }).Wrapper())
            {
                for (var i = 0; i < 1000; i++)
                {
                    await bus.PublishAsync();
                }

                await Task.Delay(300);

                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public async Task SubscriptionBuilderStackTests()
        {
            var bus = new ConcurrentMessageBus<Message>();

            using (bus.Subscribe()
                .Filter(
                    message => { message.Steps.Add("before1"); },
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
                    message => { message.Steps.Add("after2"); })
                .Handler(
                    message =>
                        {
                            message.Steps.Add("handler");
                            message.HandlerInvoked = "yes";
                        }).Wrapper())
            {
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

        private class Message
        {
            public string HandlerInvoked { get; set; }

            public string Id { get; set; }

            public List<string> Steps { get; } = new List<string>();
        }

        private class OuterMessage
        {
            public CancellationToken Token { get; set; }

            public Message Message { get; set; }
        }
    }
}