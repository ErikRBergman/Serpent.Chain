namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Concurrent
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;

    using Xunit;

    public class ConcurrentDecoratorWireUpTests
    {
        [Fact]
        public async Task ConcurrentDecoratorWireUpHandlerTests()
        {
            var handler = new ConcurrentMessageHandler();

            var func = Create.Func<Message>(b => b.SoftFireAndForget().WireUp(handler));

            await Task.Delay(100);

            for (var i = 0; i < 100; i++)
            {
                await func(new Message(), CancellationToken.None);
            }

            await Task.Delay(600);

            Assert.Equal(10, handler.NumberOfInvokations);
        }

        [Concurrent(10)]
        private class ConcurrentMessageHandler : IMessageHandler<Message>
        {
            private int numberOfInvokations;

            public int NumberOfInvokations => this.numberOfInvokations;

            public async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
            {
                await Task.Delay(500, cancellationToken);
                Interlocked.Increment(ref this.numberOfInvokations);
            }
        }

        private class Message
        {
            public string Id { get; set; }
        }
    }
}