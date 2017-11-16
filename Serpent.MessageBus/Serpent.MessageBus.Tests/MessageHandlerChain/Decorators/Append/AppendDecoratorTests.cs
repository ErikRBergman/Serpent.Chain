// ReSharper disable InconsistentNaming
// ReSharper disable AccessToModifiedClosure

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Append
{
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class AppendDecoratorTests
    {
        [Fact]
        public async Task Append_Normal_Tests()
        {
            var counter = 0;

            var func = Create.Func<int>(b => b.Append(msg => msg).Handler(value => { Interlocked.Add(ref counter, value); }));

            for (var i = 0; i < 100; i++)
            {
                await func(1 + (i % 2), CancellationToken.None);
            }

            Assert.Equal(300, counter);
        }

        [Fact]
        private static async Task Append_Async_Test()
        {
            int counter = 0;

#pragma warning disable 1998
            var func = Create.Func<int>(b => b.Append(async msg => msg).Handler(value => { Interlocked.Add(ref counter, value); }));
#pragma warning restore 1998

            for (var i = 0; i < 100; i++)
            {
                await func(1 + (i % 2), CancellationToken.None);
            }

            Assert.Equal(300, counter);
        }
    }
}