// ReSharper disable InconsistentNaming
namespace Serpent.MessageBus.Tests.Extras
{
    using System;
    using System.Threading.Tasks;

    using Serpent.MessageBus;
    using Serpent.MessageBus.Extras;

    using Xunit;

    public class PublisherBridgeTests
    {
        [Fact]
        public void PublisherBridge_Tests()
        {
            var bus = new ConcurrentMessageBus<int>();

            int count = 0;

            bus.Subscribe(s => s.Handler(m => count++));

            var bridge = new PublisherBridge<int>(bus);

            Assert.Equal(0, count);

            bridge.Publish();
            bridge.Publish();

            Assert.Equal(2, count);
        }
    }
}