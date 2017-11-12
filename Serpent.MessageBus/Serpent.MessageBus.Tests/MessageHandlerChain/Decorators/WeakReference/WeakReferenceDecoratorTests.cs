﻿namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;

    using Xunit;

    public class WeakReferenceDecoratorTests
    {
        [Fact]
        public async Task WeakReferenceDecoratorTest()
        {
            // new int[] { 1 }.SelectMany()
            var bus = new ConcurrentMessageBus<int>();

            Assert.Equal(0, bus.SubscriberCount);

            // Strong references
            bus.Subscribe(b => b.Concurrent(5).Retry(2, TimeSpan.FromSeconds(10)).Handler(new MyHandler()));

            Assert.Equal(1, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);

            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);

            // Now weak reference
            bus.Subscribe(b => b.WeakReference().Handler(new MyHandler()));

            Assert.Equal(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);

            // Now weak reference with more decorators
            bus.Subscribe(b => b.WeakReference().Delay(50).Concurrent(10).Handler(new MyHandler()));

            Assert.Equal(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);

            // Weak reference with more decorators first
            bus.Subscribe(b => b.Delay(50).Concurrent(10).WeakReference().Delay(50).Concurrent(10).Handler(new MyHandler()));

            Assert.Equal(2, bus.SubscriberCount);
            GC.Collect(2, GCCollectionMode.Forced);
            await bus.PublishAsync();

            Assert.Equal(1, bus.SubscriberCount);
        }
    }

    internal class MyHandler : IMessageHandler<int>
    {
        public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}