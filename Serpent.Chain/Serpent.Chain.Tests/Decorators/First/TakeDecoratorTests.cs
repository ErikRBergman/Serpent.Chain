// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.First
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Notification;

    using Xunit;

    public class FirstDecoratorTests
    {
        [Fact]
        public void FirstAsyncDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuilderNotifier();
            var services = new ChainBuilderSetupServices(notification);

            var chain = new ChainBuilder<int>().First(m => Task.FromResult(m == 2)).Handler(m => count++).BuildFunc(services);

            // The notification is disposed when the Take handler has processed 2 messages, trying to process the 3rd
            Assert.False(notification.IsDisposed);
            Assert.Equal(0, count);

            chain(1, CancellationToken.None);

            Assert.Equal(0, count);

            chain(2, CancellationToken.None);

            Assert.Equal(1, count);

            chain(3, CancellationToken.None);

            Assert.True(notification.IsDisposed);

            chain(4, CancellationToken.None);

            // Skip the rest - still 2
            Assert.Equal(1, count);
        }

        [Fact]
        public void FirstDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuilderNotifier();
            var services = new ChainBuilderSetupServices(notification);

            var chain = new ChainBuilder<int>().First(m => m == 2).Handler(m => count++).BuildFunc(services);

            // The notification is disposed when the Take handler has processed 2 messages, trying to process the 3rd
            Assert.False(notification.IsDisposed);
            Assert.Equal(0, count);

            chain(1, CancellationToken.None);

            Assert.Equal(0, count);

            chain(2, CancellationToken.None);

            Assert.Equal(1, count);

            chain(3, CancellationToken.None);

            Assert.True(notification.IsDisposed);

            chain(4, CancellationToken.None);

            // Skip the rest - still 2
            Assert.Equal(1, count);
        }
    }
}