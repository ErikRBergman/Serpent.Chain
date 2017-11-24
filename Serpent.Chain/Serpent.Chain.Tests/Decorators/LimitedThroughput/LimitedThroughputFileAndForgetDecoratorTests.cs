// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.LimitedThroughput
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class LimitedThroughputFileAndForgetDecoratorTests
    {
        [Fact]
        public async Task LimitedThroughputFireAndForget_Tests()
        {
            var count = 0;

            var func = Create.SimpleFunc<int>(b => b
                .LimitedThroughputFireAndForget(10, TimeSpan.FromMilliseconds(1000))
                .Handler(
                    msg =>
                        {
                            Interlocked.Increment(ref count);
                        }));

            for (var i = 0; i < 70; i++)
            {
                await func(i);
            }

            await Task.Delay(500);

            Assert.Equal(10, count);

            await Task.Delay(1000);

            Assert.Equal(20, count);

            await Task.Delay(4000);
            Assert.Equal(60, count);
        }
    }
}