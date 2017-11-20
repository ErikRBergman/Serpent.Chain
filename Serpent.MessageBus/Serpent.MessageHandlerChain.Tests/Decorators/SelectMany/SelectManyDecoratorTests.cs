// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.SelectMany
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Notification;
    using Serpent.MessageHandlerChain.Tests.Decorators.First;

    using Xunit;

    public class SelectManyDecoratorTests
    {
        [Fact]
        public void SelectManyAsyncDecorator_Test()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);
            var builder = new MessageHandlerChainBuilder<OuterMessage>();
            builder.SelectMany(m => Task.FromResult(m.Messages)).Skip(2).Take(2).Handler(m => count++);
            var chain = builder.BuildFunc(services);

            Assert.False(notification.IsDisposed);
            Assert.Equal(0, count);

            chain(
                new OuterMessage
                    {
                        Messages = new[] { 1, 2, 3 }
                    },
                CancellationToken.None);

            Assert.False(notification.IsDisposed);
            Assert.Equal(1, count);

            chain(
                new OuterMessage
                    {
                        Messages = new[] { 4, 5, 6 }
                    },
                CancellationToken.None);

            Assert.Equal(2, count);
            Assert.True(notification.IsDisposed);
        }

        [Fact]
        public void SelectManyDecorator_Test()
        {
            var count = 0;

            var notification = new TestChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(notification);
            var builder = new MessageHandlerChainBuilder<OuterMessage>();
            builder.SelectMany(m => m.Messages).Skip(2).Take(2).Handler(m => count++);
            var chain = builder.BuildFunc(services);

            Assert.False(notification.IsDisposed);
            Assert.Equal(0, count);

            chain(
                new OuterMessage
                    {
                        Messages = new[] { 1, 2, 3 }
                    },
                CancellationToken.None);

            Assert.False(notification.IsDisposed);
            Assert.Equal(1, count);

            chain(
                new OuterMessage
                    {
                        Messages = new[] { 4, 5, 6 }
                    },
                CancellationToken.None);

            Assert.Equal(2, count);
            Assert.True(notification.IsDisposed);
        }

        private class OuterMessage
        {
            public IEnumerable<int> Messages { get; set; }
        }
    }
}