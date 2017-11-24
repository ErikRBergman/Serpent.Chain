// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.SoftFireAndForget
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.SoftFireAndForget;
    using Serpent.Chain.Interfaces;

    using Xunit;

    public class SoftFireAndForgetDecoratorTests
    {
        [Fact]
        public async Task SoftTestFireAndForget_Async_Test()
        {
            var count = 0;

            var bus = Create.SimpleFunc<int>(
                b => b.SoftFireAndForget()
                    .Handler(
                        async msgz =>
                            {
                                await Task.Delay(1000);
                                Interlocked.Increment(ref count);
                            }));

            await bus(0);
            Assert.NotEqual(1, count);

            await Task.Delay(2000);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task SoftTestFireAndForget_Attribute_WireUp_Test()
        {
            var handler = new SoftFireAndForgetHandler();

            var bus = Create.SimpleFunc<int>(b => b.WireUp(handler));

            await bus(0);
            Assert.NotEqual(1, handler.Count);

            await Task.Delay(2000);

            Assert.Equal(1, handler.Count);
        }

        [Fact]
        public async Task SoftTestFireAndForget_Configuration_Async_WireUp_Test()
        {
            var count = 0;

            var wu = new SoftFireAndForgetWireUp();

            var configObject = wu.CreateConfigurationFromDefaultValue(null);
            IEnumerable<object> objects = new[] { configObject };

            var bus = Create.SimpleFunc<int>(
                b => b.WireUp(objects)
                    .Handler(
                        async _ =>
                            {
                                await Task.Delay(1000);
                                Interlocked.Increment(ref count);
                            }));

            await bus(0);
            Assert.NotEqual(1, count);

            await Task.Delay(2000);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task SoftTestFireAndForget_Sync_Test()
        {
            var count = 0;

            var bus = Create.SimpleFunc<int>(
                b => b.SoftFireAndForget()
                    .Handler(
                        _ =>
                            {
                                Interlocked.Increment(ref count);
                                return Task.CompletedTask;
                            }));

            await bus(0);
            Assert.Equal(1, count);
        }

        [SoftFireAndForget]
        public class SoftFireAndForgetHandler : IMessageHandler<int>
        {
            private int count;

            public int Count => this.count;

            public async Task HandleMessageAsync(int message, CancellationToken cancellationToken)
            {
                await Task.Delay(1000);
                Interlocked.Increment(ref this.count);
            }
        }
    }
}