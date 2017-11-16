// ReSharper disable InconsistentNaming
namespace Serpent.MessageBus.Tests.Extras
{
    using System;
    using System.Threading.Tasks;

    using Serpent.MessageBus;
    using Serpent.MessageBus.Extras;

    using Xunit;

    public class SubscriptionsBridgeTests
    {
        [Fact]
        public void SubscriptionsBridge_Tests()
        {
            var bus = new ConcurrentMessageBus<int>();

            int count = 0;

            var bridge = new SubscriptionsBridge<int>(bus);

            bridge.Subscribe(s => s.Handler(m => count++));

            Assert.Equal(0, count);

            bus.Publish();
            bus.Publish();

            Assert.Equal(2, count);
        }
    }
}