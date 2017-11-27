// ReSharper disable InconsistentNaming

#pragma warning disable 4014
namespace Serpent.Chain.Tests.Decorators.Semaphore
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Semaphore;
    using Serpent.Chain.Exceptions;

    using Xunit;

    public class SemaphoreWithKeyDecoratorTests
    {
        [Fact]
        public async Task SemaphoreWithKeyDecorator_MultipleConcurrency_Tests()
        {
            var count = 0;

            var func = Create.SimpleFunc<KeyValuePair<int, string>>(
                b => b
                    .SoftFireAndForget()
                    .Semaphore(c => c.MaxNumberOfConcurrentMessages(2).KeySelector(m => m.Key))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(500);
                                Interlocked.Increment(ref count);
                            }));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            func(new KeyValuePair<int, string>(1, "One"));
            func(new KeyValuePair<int, string>(1, "One"));
            func(new KeyValuePair<int, string>(1, "One"));
            func(new KeyValuePair<int, string>(1, "One"));

            func(new KeyValuePair<int, string>(2, "Two"));
            func(new KeyValuePair<int, string>(2, "Two"));
            func(new KeyValuePair<int, string>(2, "Two"));
            func(new KeyValuePair<int, string>(2, "Two"));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await Task.Delay(200);
            Assert.Equal(0, count);

            await Task.Delay(600);
            Assert.Equal(4, count);

            await Task.Delay(500);
            Assert.Equal(8, count);
        }

        [Fact]
        public async Task SemaphoreWithKeyDecorator_SingleConcurrency_Tests()
        {
            var count = 0;

            var func = Create.SimpleFunc<KeyValuePair<int, string>>(
                b => b
                    .FireAndForget()
                    .Semaphore(c => c.KeySelector(m => m.Key))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(500);
                                Interlocked.Increment(ref count);
                            }));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            func(new KeyValuePair<int, string>(1, "One"));
            func(new KeyValuePair<int, string>(1, "One"));
            func(new KeyValuePair<int, string>(1, "One"));
            func(new KeyValuePair<int, string>(1, "One"));

            func(new KeyValuePair<int, string>(2, "Two"));
            func(new KeyValuePair<int, string>(2, "Two"));
            func(new KeyValuePair<int, string>(2, "Two"));
            func(new KeyValuePair<int, string>(2, "Two"));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await Task.Delay(200);
            Assert.Equal(0, count);

            await Task.Delay(600);
            Assert.Equal(2, count);

            await Task.Delay(500);
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task SemaphoreWithKeyDecorator_SingleConcurrency_KeySelector_EqualityComparer_Tests()
        {
            var count = 0;

            var func = Create.SimpleFunc<string>(
                b => b
                    .FireAndForget()
                    .Semaphore(c => c.KeySelector(m => m).EqualityComparer(StringComparer.OrdinalIgnoreCase))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(500);
                                Interlocked.Increment(ref count);
                            }));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            func("A");
            func("a");
            func("a");
            func("a");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await Task.Delay(200);
            Assert.Equal(0, count);

            await Task.Delay(600);
            Assert.Equal(1, count);

            await Task.Delay(500);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task SemaphoreWithKeyDecorator_SingleConcurrency_EqualityComparer_KeySelector_Tests()
        {
            var count = 0;

            var func = Create.SimpleFunc<string>(
                b => b
                    .FireAndForget()
                    .Semaphore(c => c.EqualityComparer(StringComparer.OrdinalIgnoreCase).KeySelector(m => m))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(500);
                                Interlocked.Increment(ref count);
                            }));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            func("A");
            func("a");
            func("a");
            func("a");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await Task.Delay(200);
            Assert.Equal(0, count);

            await Task.Delay(600);
            Assert.Equal(1, count);

            await Task.Delay(500);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task SemaphoreWithKeyDecorator_SingleConcurrency_KeySemaphore_Tests()
        {
            var count = 0;

            var keySemaphore = new KeySemaphore<string>(1, StringComparer.OrdinalIgnoreCase);

            var func = Create.SimpleFunc<string>(
                b => b
                    .FireAndForget()
                    .Semaphore(c => c.KeySemaphore(keySemaphore).KeySelector(m => m))
                    .Handler(
                        async m =>
                            {
                                await Task.Delay(500);
                                Interlocked.Increment(ref count);
                            }));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            func("A");
            func("a");
            func("a");
            func("a");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await Task.Delay(200);
            Assert.Equal(0, count);

            await Task.Delay(600);
            Assert.Equal(1, count);

            await Task.Delay(500);
            Assert.Equal(2, count);
        }

        [Fact]
        public void SemaphoreWithKeyDecorator_SingleConcurrency_KeySemaphore_No_KeySelector_Tests()
        {
            var keySemaphore = new KeySemaphore<string>(1, StringComparer.OrdinalIgnoreCase);

            Assert.Throws<KeySelectorMissingException>(
                () =>
                    {
                        Create.SimpleFunc<string>(b => b.FireAndForget().Semaphore(c => c.KeySemaphore(keySemaphore)).Handler(async m => { await Task.Delay(500); }));
                    });
        }
    }
}