// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.WeakReference;
    using Serpent.Chain.Interfaces;

    using Xunit;

    public class WeakReferenceDecoratorTests
    {
        [Fact]
        public async Task WeakReferenceDecorator_WireUp_Attribute_Test()
        {
            var isDisposed = false;

            var chain = Create.CreateChain<int>(b => b.WireUp(new WeakReferenceAttributeHandler()), () => isDisposed = true);
            await chain.ChainFunc(0, CancellationToken.None);

            Assert.False(isDisposed);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.False(isDisposed);

            await chain.ChainFunc(0, CancellationToken.None);

            Assert.True(isDisposed);
        }

        [Fact]
        public async Task WeakReferenceDecorator_WireUp_Configuration_Test()
        {
            var isDisposed = false;

            var wireUp = new WeakReferenceWireUp();

            var config = wireUp.CreateConfigurationFromDefaultValue(null);

            var chain = Create.CreateChain<int>(b => b.WireUp(new WeakReferenceHandler(), new[] { config }), () => isDisposed = true);

            Assert.False(isDisposed);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.False(isDisposed);

            await chain.ChainFunc(0, CancellationToken.None);

            Assert.True(isDisposed);
        }

        [Fact]
        public async Task WeakReferenceDecoratorTest()
        {
            var isDisposed = false;

            var chain = Create.CreateChain<int>(b => b.WeakReference(new WeakReferenceHandler()), () => isDisposed = true);
            await chain.ChainFunc(0, CancellationToken.None);

            Assert.False(isDisposed);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.False(isDisposed);

            await chain.ChainFunc(0, CancellationToken.None);

            Assert.True(isDisposed);
        }

        [WeakReference]
        private class WeakReferenceAttributeHandler : IMessageHandler<int>
        {
            public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        private class WeakReferenceHandler : IMessageHandler<int>
        {
            private readonly Random random = new Random();

            public int GetRandomValue()
            {
                return this.random.Next(1, 20);
            }

            public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}