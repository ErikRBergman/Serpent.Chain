// ReSharper disable InconsistentNaming
namespace Serpent.Common.MessageBus.Tests.MessageHandlerChain
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppendDecoratorTests
    {
        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Normal()
        {
            var bus = new ConcurrentMessageBus<int>();
            var items = new List<int>();

            bus.Subscribe().Append(msg => 1).Handler(
                msg =>
                    {
                        lock (items)
                        {
                            items.Add(msg);
                        }
                    });

            await bus.PublishAsync(0);

            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(0, items[0]);
            Assert.AreEqual(1, items[1]);
        }

        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Normal_Async()
        {
            var bus = new ConcurrentMessageBus<int>();
            var items = new List<int>();

            bus.Subscribe().Append(async msg =>
                {
                    await Task.Delay(10);
                    return 1;
                }).Handler(
                msg =>
                    {
                        lock (items)
                        {
                            items.Add(msg);
                        }
                    });

            await bus.PublishAsync(0);

            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(0, items[0]);
            Assert.AreEqual(1, items[1]);
        }
    }
}