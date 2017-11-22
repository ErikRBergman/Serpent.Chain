namespace Serpent.MessageHandlerChain.Tests.Decorators.Concurrent
{
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class ConcurrentFireAndForgetDecoratorTests
    {
        [Fact]
        public async Task ConcurrentFireAndForgetDecoratorHandlerTests()
        {
            var counter = 0;

            var func = Create.SimpleFunc<int>(
                b => b
                    .ConcurrentFireAndForget(10)
                    .Handler(
                        async message =>
                            {
                                await Task.Delay(500);
                                Interlocked.Increment(ref counter);
                            }));

            for (var i = 0; i < 20; i++)
            {
                await func(0);
            }

            await Task.Delay(800);

            Assert.Equal(10, counter);
        }
    }
}