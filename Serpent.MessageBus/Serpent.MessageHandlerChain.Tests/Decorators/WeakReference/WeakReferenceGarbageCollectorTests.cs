// ReSharper disable InconsistentNaming
namespace Serpent.MessageHandlerChain.Tests.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.WeakReference;

    using Xunit;

    public class WeakReferenceGarbageCollectorTests
    {
        [Fact]
        public async Task WeakReferenceDecorator_WeakReferenceGarbageCollector_Test()
        {
            var isDisposed = false;

            var gc = new WeakReferenceGarbageCollector(TimeSpan.FromMilliseconds(100));

            var chain = Create.CreateChain<int>(b => b.WeakReference(gc).Handler(m => { }), () => isDisposed = true);

            Assert.False(isDisposed);
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.False(isDisposed);

            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            await chain.MessageHandlerChainFunc(0, CancellationToken.None);

            Assert.True(isDisposed);
        }
    }
}