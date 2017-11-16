

// ReSharper disable InconsistentNaming
namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Semaphore
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class SemaphoreWithKeyDecoratorTests
    {
        private const int delayMultiplier = 5;

        [Fact]
        public async Task SemaphoreWithKeyDecorator_SingleConcurrency_Tests()
        {
            var count = 0;

            var func = Create.Func<KeyValuePair<int, string>>(b => b.Semaphore(c => c.KeySelector(m => m.Key)).Handler(
                async m =>
                    {
                        await Task.Delay(delayMultiplier * 100);
                        Interlocked.Increment(ref count);
                    }));


            var t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));

            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));


            await Task.Delay(50);

            Assert.Equal(0, count);

            await Task.Delay(delayMultiplier * 100);

            Assert.Equal(2, count);

            await Task.Delay(delayMultiplier * 500);

            Assert.Equal(8, count);
        }

        [Fact]
        public async Task SemaphoreWithKeyDecorator_MultipleConcurrency_Tests()
        {
            var count = 0;

            var func = Create.Func<KeyValuePair<int, string>>(b => b.Semaphore(c => c.MaxNumberOfConcurrentMessages(2).KeySelector(m => m.Key)).Handler(
                async m =>
                    {
                        await Task.Delay(delayMultiplier * 100);
                        Interlocked.Increment(ref count);
                    }));

            var t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(1, "One"), CancellationToken.None));

            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));
            t = Task.Run(() => func(new KeyValuePair<int, string>(2, "Two"), CancellationToken.None));


            await Task.Delay(50);

            Assert.Equal(0, count);

            await Task.Delay(delayMultiplier * 100);

            Assert.Equal(4, count);

            await Task.Delay(delayMultiplier * 300);

            Assert.Equal(8, count);

        }

    }
}