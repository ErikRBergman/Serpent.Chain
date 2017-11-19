// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.LimitedThroughput
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.LimitedThroughput;

    using Xunit;

    public class LimitedThroughputWireUpTests
    {
        private const int DelayMultiplier = 10;

        [Fact]
        public async Task LimitedThroughput_WireUp_Attribute_Test()
        {
            var handler = new LimitedThroughputMessageHandler();

            var func = Create.Func<LimitedThroughputTestMessage>(s => s.SoftFireAndForget().WireUp(handler));

            for (int i = 0; i < 100; i++)
            {
                await func(new LimitedThroughputTestMessage("1"), CancellationToken.None);
            }

            await Task.Delay(DelayMultiplier * 110);

            Assert.Equal(20, handler.Count);

            await Task.Delay(DelayMultiplier * 410);
            Assert.Equal(60, handler.Count);
        }

        [Fact]
        public async Task TestWireUpFromConfiguration()
        {
            var wireUp = new LimitedThroughputWireUp();

            var config = wireUp.CreateConfigurationFromDefaultValue("10");

            var limitedThroughputConfiguration = config as LimitedThroughputConfiguration;
            Assert.NotNull(limitedThroughputConfiguration);

            Assert.Equal(10, limitedThroughputConfiguration.MaxNumberOfMessagesPerPeriod);
            Assert.Equal(TimeSpan.FromSeconds(1), limitedThroughputConfiguration.Period);

            var handler = new LimitedThroughputMessageHandler();

            var func = Create.Func<LimitedThroughputTestMessage>(b => b.SoftFireAndForget().WireUp(handler, new[] { config }));

            Assert.Equal(0, handler.Count);

            for (int i = 0; i < 100; i++)
            {
#pragma warning disable 4014
                func(new LimitedThroughputTestMessage("1"), CancellationToken.None);
#pragma warning restore 4014
            }

            await Task.Delay(DelayMultiplier * 110);

            Assert.Equal(20, handler.Count);

            await Task.Delay(DelayMultiplier * 410);
            Assert.Equal(60, handler.Count);
        }
    }
}