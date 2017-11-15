// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.First
{
    using System.Threading;

    using Serpent.MessageBus.MessageHandlerChain;

    using Xunit;

    public class FirstDecoratorTests
    {
        [Fact]
        public void FirstDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);

            var chain = new MessageHandlerChainBuilder<int>().First(m => m == 2).Handler(m => count++).BuildFunc(services);

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
        public void FirstAsyncDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);

            var chain = new MessageHandlerChainBuilder<int>().First(async m => m == 2).Handler(m => count++).BuildFunc(services);

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