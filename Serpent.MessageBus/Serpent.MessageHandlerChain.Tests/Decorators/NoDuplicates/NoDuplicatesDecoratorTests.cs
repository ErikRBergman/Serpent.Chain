// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.NoDuplicates
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

            var func = Create.SimpleFunc<Message>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.KeySelector(m => m.Id))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("A"));

            await Task.Delay(300);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_KeySelector_PassThrough_Test()
        {
            var count = 0;

            var func = Create.SimpleFunc<Message>(
                s => s
                    .NoDuplicates(b => b.KeySelector(m => m.Id))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("A"));

            Assert.Equal(5, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_No_KeySelector_Test()
        {
            var count = 0;

            var func = Create.SimpleFunc<string>(
                s => s.SoftFireAndForget()
                    .NoDuplicates()
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func("a");
            await func("a");
            await func("a");
            await func("a");
            await func("A");

            await Task.Delay(300);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_EqualityComparer_Test()
        {
            var count = 0;

            var func = Create.SimpleFunc<string>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.EqualityComparer(StringComparer.OrdinalIgnoreCase))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func("a");
            await func("a");
            await func("a");
            await func("a");
            await func("A");

            await Task.Delay(300);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_KeySelector_EqualityComparer_Test_1()
        {
            var count = 0;

            var func = Create.SimpleFunc<Message>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.EqualityComparer(StringComparer.Ordinal).KeySelector(m => m.Id))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("A"));

            await Task.Delay(300);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task NoDuplicates_Builder_KeySelector_EqualityComparer_Test_2()
        {
            var count = 0;

            var func = Create.SimpleFunc<Message>(
                s => s.SoftFireAndForget()
                    .NoDuplicates(b => b.KeySelector(m => m.Id).EqualityComparer(StringComparer.OrdinalIgnoreCase))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(200);
                                Interlocked.Increment(ref count);
                            }));

            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("a"));
            await func(new Message("A"));

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