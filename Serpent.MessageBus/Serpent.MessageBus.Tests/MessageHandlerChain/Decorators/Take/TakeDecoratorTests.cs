// ReSharper disable InconsistentNaming
namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Take
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Serpent.MessageBus.Interfaces;
    using Serpent.MessageBus.MessageHandlerChain;

    using Xunit;

    public class TakeDecoratorTests
    {
        [Fact]
        public void TakeDecorator_Tests()
        {
            var count = 0;

            var notification = new MyNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);

            var chain = new MessageHandlerChainBuilder<int>().Take(2).Handler(m => count++).BuildFunc(services);

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

        private class MyNotification : IMessageHandlerChainBuildNotification, IMessageHandlerChain
        {
            public bool IsDisposed { get; private set; }

            public void AddNotification(Action<IMessageHandlerChain> messageHandlerChain)
            {
                messageHandlerChain(this);
            }

            public void Dispose()
            {
                this.IsDisposed = true;
            }
        }
    }
}