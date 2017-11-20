// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.LimitedThroughput
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class LimitedThroughputDecoratorTests
    {
        [Fact]
        public async Task LimitedThroughput_Tests()
        {
            var count = 0;

            var bus = Create.SimpleFunc<int>(
                b => b
                    .SoftFireAndForget()
                    .LimitedThroughput(10, TimeSpan.FromMilliseconds(1000))
                    .Handler(msg => { Interlocked.Increment(ref count); }));

                for (var i = 0; i < 70; i++)
                {
                    await bus(i);
                }

                await Task.Delay(1500);

                Assert.Equal(20, count);

                await Task.Delay(4000);
                Assert.Equal(60, count);
        }
    }
}