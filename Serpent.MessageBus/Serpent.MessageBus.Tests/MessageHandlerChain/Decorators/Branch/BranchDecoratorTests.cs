// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Branch
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BranchDecoratorTests
    {
        [TestMethod]
        public async Task BranchDecorator_Subscribe_Normal_Tests()
        {
            var bus = new ConcurrentMessageBus<int>();
            var items = new HashSet<int>();

            bus.Subscribe(b => b
                .Branch(
                    branch => branch.Handler(msg =>
                        {
                            lock (items)
                            {
                                items.Add(1);
                            }
                        }),
                    branch => branch.Handler(msg =>
                        {
                            lock (items)
                            {
                                items.Add(2);
                            }
                        })));

            await bus.PublishAsync(1);

            Assert.AreEqual(2, items.Count);

            Assert.IsTrue(items.Contains(1));
            Assert.IsTrue(items.Contains(2));
        }
    }
}