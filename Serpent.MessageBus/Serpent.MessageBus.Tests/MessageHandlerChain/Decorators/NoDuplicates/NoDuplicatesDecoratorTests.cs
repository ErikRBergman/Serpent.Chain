// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.NoDuplicates
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class NoDuplicatesDecoratorTests
    {
        [Fact]
        public async Task NoDuplicates_Builder_KeySelector_Test()
        {
            var count = 0;

            var func = Create.Func<Message>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.KeySelector(m => m.Id))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("A"), CancellationToken.None);

            await Task.Delay(300);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_No_KeySelector_Test()
        {
            var count = 0;

            var func = Create.Func<string>(
                s => s.SoftFireAndForget()
                    .NoDuplicates()
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func("a", CancellationToken.None);
            await func("a", CancellationToken.None);
            await func("a", CancellationToken.None);
            await func("a", CancellationToken.None);
            await func("A", CancellationToken.None);

            await Task.Delay(300);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_EqualityComparer_Test()
        {
            var count = 0;

            var func = Create.Func<string>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.EqualityComparer(StringComparer.OrdinalIgnoreCase))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func("a", CancellationToken.None);
            await func("a", CancellationToken.None);
            await func("a", CancellationToken.None);
            await func("a", CancellationToken.None);
            await func("A", CancellationToken.None);

            await Task.Delay(300);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_KeySelector_EqualityComparer_Test_1()
        {
            var count = 0;

            var func = Create.Func<Message>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.EqualityComparer(StringComparer.Ordinal).KeySelector(m => m.Id))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("A"), CancellationToken.None);

            await Task.Delay(300);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_KeySelector_EqualityComparer_Test_2()
        {
            var count = 0;

            var func = Create.Func<Message>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.KeySelector(m => m.Id).EqualityComparer(StringComparer.OrdinalIgnoreCase))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("a"), CancellationToken.None);
            await func(new Message("A"), CancellationToken.None);

            await Task.Delay(300);

            Assert.Equal(1, count);
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