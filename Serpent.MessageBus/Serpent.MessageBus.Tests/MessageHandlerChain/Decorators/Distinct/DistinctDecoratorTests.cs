// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Distinct
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.Distinct;

    using Xunit;

    public class DistinctDecoratorTests
    {
        [Fact]
        public async Task Distinct_Builder_AsyncKeySelector_EqualityComparer_Test()
        {
            {
                var count = 0;

                var func = Create.Func<string>(s => s.Distinct(b => b.KeySelector(Task.FromResult).EqualityComparer(StringComparer.Ordinal)).Handler(m => count++));

                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("A", CancellationToken.None);

                Assert.Equal(2, count);
            }

            {
                var count = 0;

                var func = Create.Func<string>(s => s.Distinct(b => b.KeySelector(Task.FromResult).EqualityComparer(StringComparer.OrdinalIgnoreCase)).Handler(m => count++));

                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("A", CancellationToken.None);

                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task Distinct_Builder_AsyncKeySelector_Test()
        {
            var count = 0;

            var func = Create.Func<DistinctTestMessage>(s => s.Distinct(b => b.KeySelector(m => Task.FromResult(m.Id))).Handler(m => count++));

            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("2"), CancellationToken.None);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Distinct_Builder_EqualityComparer_Test()
        {
            {
                var count = 0;

                var func = Create.Func<string>(s => s.Distinct(b => b.EqualityComparer(StringComparer.Ordinal)).Handler(m => count++));

                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("A", CancellationToken.None);

                Assert.Equal(2, count);
            }

            {
                var count = 0;

                var func = Create.Func<string>(s => s.Distinct(b => b.EqualityComparer(StringComparer.OrdinalIgnoreCase)).Handler(m => count++));

                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("a", CancellationToken.None);
                await func("A", CancellationToken.None);

                Assert.Equal(1, count);
            }
        }

        [Fact]
        public void Distinct_Builder_EqualityComparer_NoKeySelector_Test()
        {
            Assert.Throws<KeySelectorMissingException>(() => Create.Func<int>(s => s.Distinct(b => b.EqualityComparer(StringComparer.Ordinal)).Handler(m => { })));
        }

        [Fact]
        public async Task Distinct_Builder_KeySelector_Test()
        {
            var count = 0;

            var func = Create.Func<DistinctTestMessage>(s => s.Distinct(b => b.KeySelector(m => m.Id)).Handler(m => count++));

            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("2"), CancellationToken.None);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Distinct_KeySelector_Test()
        {
            var count = 0;

            var func = Create.Func<DistinctTestMessage>(s => s.Distinct(m => m.Id).Handler(m => count++));

            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("1"), CancellationToken.None);
            await func(new DistinctTestMessage("2"), CancellationToken.None);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Distinct_ValueType_NoKeySelector_Test()
        {
            var count = 0;

            var func = Create.Func<int>(s => s.Distinct().Handler(m => count++));

            await func(1, CancellationToken.None);
            await func(1, CancellationToken.None);
            await func(1, CancellationToken.None);
            await func(1, CancellationToken.None);
            await func(1, CancellationToken.None);
            await func(2, CancellationToken.None);

            Assert.Equal(2, count);
        }
    }
}