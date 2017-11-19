namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Concurrent
{
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class ConcurrentDecoratorTests
    {
        [Fact]
        public async Task ConcurrentDecoratorHandlerTests()
        {
            var counter = 0;

            var func = Create.SimpleFunc<Message>(
                b => b.SoftFireAndForget()
                    .Concurrent(10)
                    .Handler(
                        async message =>
                            {
                                await Task.Delay(500);
                                Interlocked.Increment(ref counter);
                            }));

            await Task.Delay(100);

            for (var i = 0; i < 100; i++)
            {
                await func(new Message());
            }

            await Task.Delay(600);

            Assert.Equal(10, counter);
        }

        private class Message
        {
        }
    }
}