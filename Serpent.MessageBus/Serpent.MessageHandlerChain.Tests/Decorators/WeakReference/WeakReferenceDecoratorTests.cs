// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.WeakReference;
    using Serpent.MessageHandlerChain.Interfaces;

    using Xunit;

    public class WeakReferenceDecoratorTests
    {
        [Fact]
        public async Task WeakReferenceDecorator_WireUp_Attribute_Test()
        {
            var isDisposed = false;

            var handler = new WeakReferenceAttributeHandler();
            var chain = Create.CreateChain<int>(b => b.WireUp(handler), () => isDisposed = true);
            await chain.MessageHandlerChainFunc(0, CancellationToken.None);

            Assert.False(isDisposed);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.False(isDisposed);

            await chain.MessageHandlerChainFunc(0, CancellationToken.None);

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

            await chain.MessageHandlerChainFunc(0, CancellationToken.None);

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
            public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}