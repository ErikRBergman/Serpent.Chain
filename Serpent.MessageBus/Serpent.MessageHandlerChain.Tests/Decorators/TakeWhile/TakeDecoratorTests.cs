// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.TakeWhile
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Notification;
    using Serpent.MessageHandlerChain.Tests.Decorators.First;

    using Xunit;

    public class TakeWhileDecoratorTests
    {
        [Fact]
        public void TakeWhileAsyncDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);

            var chain = new MessageHandlerChainBuilder<int>().TakeWhile(m => Task.FromResult(m <= 2)).Handler(m => count++).BuildFunc(services);

            // The notification is disposed when the Take handler has processed 2 messages, trying to process the 3rd
            Assert.False(notification.IsDisposed);
            Assert.Equal(0, count);

            chain(1, CancellationToken.None);
            chain(2, CancellationToken.None);

            // 2 should be skipped taken
            Assert.Equal(2, count);

            chain(3, CancellationToken.None);

            Assert.True(notification.IsDisposed);

            chain(4, CancellationToken.None);

            // Skip the rest - still 2
            Assert.Equal(2, count);
        }

        [Fact]
        public void TakeWhileDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);

            var chain = new MessageHandlerChainBuilder<int>().TakeWhile(m => m <= 2).Handler(m => count++).BuildFunc(services);

            // The notification is disposed when the Take handler has processed 2 messages, trying to process the 3rd
            Assert.False(notification.IsDisposed);
            Assert.Equal(0, count);

            chain(1, CancellationToken.None);
            chain(2, CancellationToken.None);

            // 2 should be skipped taken
            Assert.Equal(2, count);

            chain(3, CancellationToken.None);

            Assert.True(notification.IsDisposed);

            chain(4, CancellationToken.None);

            // Skip the rest - still 2
            Assert.Equal(2, count);
        }
    }
}