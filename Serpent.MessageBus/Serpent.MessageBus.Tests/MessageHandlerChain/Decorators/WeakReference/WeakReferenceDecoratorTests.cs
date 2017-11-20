// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.WeakReference;

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

        [Fact]
        public async Task WeakReferenceDecoratorTest()
        {
            // new int[] { 1 }.SelectMany()
            var bus = new ConcurrentMessageBus<int>();

            Assert.Equal(0, bus.SubscriberCount);

            // Strong references
            bus.Subscribe(b => b.Concurrent(5).Retry(2, TimeSpan.FromSeconds(10)).Handler(new WeakReferenceAttributeHandler()));

            Assert.Equal(1, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);

            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);

            // Now weak reference
            bus.Subscribe(b => b.WeakReference().Handler(new WeakReferenceHandler()));

            Assert.Equal(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);

            // Now weak reference with more decorators
            bus.Subscribe(b => b.WeakReference().Delay(50).Concurrent(10).Handler(new WeakReferenceHandler()));

            Assert.Equal(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);

            // Weak reference with more decorators first
            bus.Subscribe(b => b.Delay(50).Concurrent(10).WeakReference().Delay(50).Concurrent(10).Handler(new WeakReferenceHandler()));

            Assert.Equal(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);
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