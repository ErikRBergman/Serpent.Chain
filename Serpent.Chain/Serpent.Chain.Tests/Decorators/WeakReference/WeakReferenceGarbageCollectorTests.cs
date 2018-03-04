// ReSharper disable InconsistentNaming
namespace Serpent.Chain.Tests.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.WeakReference;
    using Serpent.Chain.Interfaces;

    using Xunit;

    public class WeakReferenceGarbageCollectorTests
    {
        [Fact]
        public async Task WeakReferenceDecorator_WeakReferenceGarbageCollector_Test()
        {
            var isDisposed = false;

            var gc = new WeakReferenceGarbageCollector(TimeSpan.FromMilliseconds(100));

            var chain = Create.CreateChain<int>(b => b.WeakReference(new MessageHandler(), gc), () => isDisposed = true);

            Assert.False(isDisposed);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.False(isDisposed);

            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            await chain.ChainFunc(0, CancellationToken.None);

            Assert.True(isDisposed);
        }

        private class MessageHandler : IMessageHandler<int>
        {
            public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}