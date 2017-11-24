// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Distinct
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Exceptions;

    using Xunit;

    public class DistinctDecoratorTests
    {
        [Fact]
        public async Task Distinct_Builder_AsyncKeySelector_EqualityComparer_Test()
        {
            {
                var count = 0;

                var func = Create.SimpleFunc<string>(s => s.Distinct(b => b.KeySelector(Task.FromResult).EqualityComparer(StringComparer.Ordinal)).Handler(m => count++));

                await func("a");
                await func("a");
                await func("a");
                await func("A");

                Assert.Equal(2, count);
            }

            // Ignore case
            {
                var count = 0;

                var func = Create.SimpleFunc<string>(s => s.Distinct(b => b.KeySelector(Task.FromResult).EqualityComparer(StringComparer.OrdinalIgnoreCase)).Handler(m => count++));

                await func("a");
                await func("a");
                await func("a");
                await func("A");

                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task Distinct_Builder_AsyncKeySelector_Test()
        {
            var count = 0;

            var func = Create.SimpleFunc<DistinctTestMessage>(s => s.Distinct(b => b.KeySelector(m => Task.FromResult(m.Id))).Handler(m => count++));

            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("2"));

            Assert.Equal(2, count);
        }

        [Fact]
        public void Distinct_Builder_EqualityComparer_NoKeySelector_Test()
        {
            Assert.Throws<KeySelectorMissingException>(() => Create.SimpleFunc<int>(s => s.Distinct(b => b.EqualityComparer(StringComparer.Ordinal)).Handler(m => { })));
        }

        [Fact]
        public async Task Distinct_Builder_EqualityComparer_Test()
        {
            {
                var count = 0;

                var func = Create.SimpleFunc<string>(s => s.Distinct(b => b.EqualityComparer(StringComparer.Ordinal)).Handler(m => count++));

                await func("a");
                await func("a");
                await func("a");
                await func("A");

                Assert.Equal(2, count);
            }
            
            // ignore case
            {
                var count = 0;

                var func = Create.SimpleFunc<string>(s => s.Distinct(b => b.EqualityComparer(StringComparer.OrdinalIgnoreCase)).Handler(m => count++));

                await func("a");
                await func("a");
                await func("a");
                await func("A");

                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task Distinct_Builder_KeySelector_Test()
        {
            var count = 0;

            var func = Create.SimpleFunc<DistinctTestMessage>(s => s.Distinct(b => b.KeySelector(m => m.Id)).Handler(m => count++));

            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("2"));

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Distinct_KeySelector_Test()
        {
            var count = 0;

            var func = Create.SimpleFunc<DistinctTestMessage>(s => s.Distinct(m => m.Id).Handler(m => count++));

            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("1"));
            await func(new DistinctTestMessage("2"));

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Distinct_ValueType_NoKeySelector_Test()
        {
            var count = 0;

            var func = Create.SimpleFunc<int>(s => s.Distinct().Handler(m => count++));

            await func(1);
            await func(1);
            await func(1);
            await func(1);
            await func(1);
            await func(2);

            Assert.Equal(2, count);
        }
    }
}