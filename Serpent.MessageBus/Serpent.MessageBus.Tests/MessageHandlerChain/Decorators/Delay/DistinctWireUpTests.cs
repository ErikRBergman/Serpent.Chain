// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Delay
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;

    using Xunit;

    public class DelayWireUpTests
    {
        [Fact]
        public async Task DelayWireUp_Double_Attribute_Test()
        {
            var handler = new DelayDoubleMessageHandler();
            await TestDelayWireUpAsync(handler);
        }

        [Fact]
        public async Task DelayWireUp_TimeSpan_Attribute_Test()
        {
            var handler = new DelayMessageHandler();
            await TestDelayWireUpAsync(handler);
        }

        private static async Task TestDelayWireUpAsync<T>(T handler)
            where T : DelayMessageHandlerBase
        {
            var func = Create.SimpleFunc<int>(b => b.WireUp(handler));

            Assert.Equal(0, handler.NumberOfInvokations);

            // Don't await
#pragma warning disable 4014
            func(1);
#pragma warning restore 4014

            Assert.Equal(0, handler.NumberOfInvokations);

            await Task.Delay(300);
            await func(2);

            Assert.Equal(2, handler.NumberOfInvokations);
        }

        [Delay(0.1)]
        private class DelayDoubleMessageHandler : DelayMessageHandlerBase
        {
        }

        [Delay("00:00:00.100")]
        private class DelayMessageHandler : DelayMessageHandlerBase
        {
        }

        private class DelayMessageHandlerBase : IMessageHandler<int>
        {
            public int NumberOfInvokations { get; private set; }

            public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
            {
                this.NumberOfInvokations++;

                return Task.CompletedTask;
            }
        }
    }
}