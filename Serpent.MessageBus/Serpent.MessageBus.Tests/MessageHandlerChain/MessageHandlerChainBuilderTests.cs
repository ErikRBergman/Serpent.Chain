namespace Serpent.MessageBus.Tests.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class MessageHandlerChainBuilderTests
    {
        [Fact]
        public async Task MessageHandlerChainStackAllTest()
        {
            var bus = new ConcurrentMessageBus<Message>();

            var count = 0;

            using (bus.Subscribe(
                b => b.FireAndForget()
                    .SoftFireAndForget()
                    .NoDuplicates(message => message.Id)
                    .Concurrent(16)
                    .ConcurrentFireAndForget(16)
                    .Exception((msg, e) => Console.WriteLine(e))
                    .Where(msg => true)
                    .FireAndForget()
                    .BranchOut(
                        branch => { branch.FireAndForget().Delay(TimeSpan.FromSeconds(10)).Handler(message => { Console.WriteLine("Sub branch 1"); }); },
                        branch => { branch.FireAndForget().Delay(TimeSpan.FromSeconds(20)).Handler(message => { Console.WriteLine("Sub branch 2"); }); })
                    .Retry(5, TimeSpan.FromSeconds(5))
                    .Semaphore(1)
                    .LimitedThroughput(1, TimeSpan.FromSeconds(1))
                    .Delay(TimeSpan.FromMilliseconds(50))
                    .Select(
                        message => new OuterMessage
                                       {
                                           Message = message,
                                           Token = CancellationToken.None
                                       })
                    .Handler(
                        async message =>
                            {
                                Debug.WriteLine(DateTime.Now);
                                await Task.Delay(200);
                                message.Message.HandlerInvoked = "Sure was";
                                Interlocked.Increment(ref count);
                            })))
            {
                for (var i = 0; i < 30; i++)
                {
                    bus.Publish(new Message()
                                    {
                                        Id = "ABC"
                                    });
                }

                await Task.Delay(600);

                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task SubscriptionBuilderStackTests()
        {
            var bus = new ConcurrentMessageBus<Message>();

            using (bus.Subscribe(
                    b => b.Action(
                            c => c.Before(message => message.Steps.Add("before1"))
                                .Finally(
                                    message =>
                                        {
                                            if (message.Steps.Contains("after2"))
                                            {
                                                message.Steps.Add("after1");
                                            }
                                        }))
                        .Action(
                            c => c.Before(
                                    message =>
                                        {
                                            if (message.Steps.Contains("before1"))
                                            {
                                                message.Steps.Add("before2");
                                            }
                                        })
                                .Finally(message => message.Steps.Add("after2")))
                        .Handler(
                            message =>
                                {
                                    message.Steps.Add("handler");
                                    message.HandlerInvoked = "yes";
                                }))
                .Wrapper())
            {
                var msg = new Message();
                await bus.PublishAsync(msg);

                Assert.Equal("yes", msg.HandlerInvoked);

                Assert.Equal("before1", msg.Steps[0]);
                Assert.Equal("before2", msg.Steps[1]);

                Assert.Equal("handler", msg.Steps[2]);

                Assert.Equal("after2", msg.Steps[3]);
                Assert.Equal("after1", msg.Steps[4]);
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
            public Message Message { get; set; }

            public CancellationToken Token { get; set; }
        }
    }
}