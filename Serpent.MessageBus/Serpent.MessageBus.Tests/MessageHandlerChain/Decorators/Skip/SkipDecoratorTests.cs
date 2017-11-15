// ReSharper disable InconsistentNaming
namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Skip
{
    using Xunit;

    public class SkipDecoratorTests
    {
        [Fact]
        public void SKipDecorator_Tests()
        {
            var bus = new ConcurrentMessageBus<int>();
            var count = 0;
            bus.Subscribe(s => s.Skip(2).Handler(m => count++));

            Assert.Equal(0, count);

            bus.Publish(1);
            bus.Publish(2);

            // 2 should be skipped
            Assert.Equal(0, count);

            bus.Publish(3);
            bus.Publish(4);

            Assert.Equal(2, count);
        }
    }
}