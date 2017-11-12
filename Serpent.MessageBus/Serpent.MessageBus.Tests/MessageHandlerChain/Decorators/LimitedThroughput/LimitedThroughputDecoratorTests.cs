// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.LimitedThroughput
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class LimitedThroughputDecoratorTests
    {
        [Fact]
        public async Task LimitedThroughput_Subscription_Delay_Tests()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            var count = 0;

            using (bus.Subscribe(b => b
                .SoftFireAndForget()
                .LimitedThroughput(10, TimeSpan.FromMilliseconds(500))
                .Handler(
                    async msg =>
                        {
                            Interlocked.Increment(ref count);
                            //await Task.Delay(100);
                        })))
            {
                for (var i = 0; i < 100; i++)
                {
                    bus.Publish();
                }

                await Task.Delay(550);

                Assert.Equal(20, count);

                await Task.Delay(5 * 410);
                Assert.Equal(60, count);
            }
        }

        [Fact]
        public async Task LimitedThroughput_Subscription_Test()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            var count = 0;

            using (bus.Subscribe(b =>
                b
                    .SoftFireAndForget()
                    .LimitedThroughput(10, TimeSpan.FromMilliseconds(100))
                    .Handler(
                        msgz => Interlocked.Increment(ref count))))
            {
                for (var i = 0; i < 100; i++)
                {
                    bus.Publish();
                }

                await Task.Delay(130);

                Assert.Equal(20, count);

                await Task.Delay(430);
                Assert.Equal(60, count);
            }
        }

        private class Message1
        {
            public string Status { get; set; }
        }
    }
}