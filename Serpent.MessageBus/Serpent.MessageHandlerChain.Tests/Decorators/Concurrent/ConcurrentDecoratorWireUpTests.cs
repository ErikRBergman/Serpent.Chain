namespace Serpent.MessageHandlerChain.Tests.Decorators.Concurrent
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.Concurrent;
    using Serpent.MessageHandlerChain.Interfaces;

    using Xunit;

    public class ConcurrentDecoratorWireUpTests
    {
        [Fact]
        public async Task ConcurrentDecoratorWireUpHandlerTests()
        {
            var handler = new ConcurrentMessageHandler();

            var func = Create.SimpleFunc<int>(b => b.SoftFireAndForget().WireUp(handler));

            for (var i = 0; i < 20; i++)
            {
                await func(1);
            }

            await Task.Delay(1500);

            Assert.Equal(10, handler.NumberOfInvokations);
        }

        [Fact]
        public async Task TestWireUpFromConfiguration()
        {
            var wireUp = new ConcurrentWireUp();

            var config = wireUp.CreateConfigurationFromDefaultValue("10");

            var limitedThroughputConfiguration = config as ConcurrentConfiguration;
            Assert.NotNull(limitedThroughputConfiguration);

            Assert.Equal(10, limitedThroughputConfiguration.MaxNumberOfConcurrentMessages);

            var handler = new ConcurrentMessageHandler();

            var func = Create.SimpleFunc<int>(b => b.SoftFireAndForget().WireUp(handler, new[] { config }));

            Assert.Equal(0, handler.NumberOfInvokations);

            for (var i = 0; i < 20; i++)
            {
                await func(1);
            }

            await Task.Delay(1500);

            Assert.Equal(10, handler.NumberOfInvokations);
        }

        [Concurrent(10)]
        private class ConcurrentMessageHandler : IMessageHandler<int>
        {
            private int numberOfInvokations;

            public int NumberOfInvokations => this.numberOfInvokations;

            public async Task HandleMessageAsync(int message, CancellationToken cancellationToken)
            {
                await Task.Delay(1000, cancellationToken);
                Interlocked.Increment(ref this.numberOfInvokations);
            }
        }

        private class Message
        {
            public Message(string id)
            {
                this.Id = id;
            }

            public string Id { get; }
        }
    }
}