// ReSharper disable InconsistentNaming
namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.SkipWhile
{
    using System;
    using System.Threading;

    using Serpent.MessageBus.Interfaces;
    using Serpent.MessageBus.MessageHandlerChain;
    using Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.First;

    using Xunit;

    public class SkipWhileDecoratorTests
    {
        [Fact]
        public void SkipWhileDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);

            var chain = new MessageHandlerChainBuilder<int>().SkipWhile(m => m <= 2).Handler(m => count++).BuildFunc(services);

            // The notification is disposed when the Take handler has processed 2 messages, trying to process the 3rd
            Assert.Equal(0, count);

            chain(1, CancellationToken.None);
            chain(2, CancellationToken.None);

            // 0 should be taken
            Assert.Equal(0, count);

            chain(3, CancellationToken.None);
            chain(4, CancellationToken.None);

            // take the rest - still 2
            Assert.Equal(2, count);
        }

        [Fact]
        public void SkipWhileAsyncDecorator_Tests()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);
            var chain = new MessageHandlerChainBuilder<int>().SkipWhile(async m => m <= 2).Handler(m => count++).BuildFunc(services);

            // The notification is disposed when the Take handler has processed 2 messages, trying to process the 3rd
            Assert.False(notification.IsDisposed);
            Assert.Equal(0, count);

            chain(1, CancellationToken.None);
            chain(2, CancellationToken.None);

            // 0 should be taken
            Assert.Equal(0, count);

            chain(3, CancellationToken.None);
            chain(4, CancellationToken.None);

            // Take the rest - still 2
            Assert.Equal(2, count);
        }
    }
}