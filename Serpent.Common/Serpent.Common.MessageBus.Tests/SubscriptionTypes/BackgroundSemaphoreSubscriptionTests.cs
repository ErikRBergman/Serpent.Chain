namespace Serpent.Common.MessageBus.Tests.SubscriptionTypes
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackgroundSemaphoreSubscriptionTests
    {
        [TestMethod]
        public async Task BackgroundSemaphoreSubscriptionFuncTests()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            var counter = 0;

            using (bus.Subscribe()
                .Concurrent(10)
                .Handler(
                    async message =>
                        {
                            await Task.Delay(500);
                            Interlocked.Increment(ref counter);
                        }))
            {
                await Task.Delay(100);

                for (var i = 0; i < 100; i++)
                {
                    await bus.PublishAsync(new Message1());
                }

                await Task.Delay(600);

                Assert.AreEqual(10, counter);
            }
        }

        private class Message1
        {
        }
    }
}