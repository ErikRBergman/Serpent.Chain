// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Branch
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Xunit;

    public class BranchDecoratorTests
    {
        [Fact]
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

            Assert.Equal(2, items.Count);

            Assert.Contains(1, items);
            Assert.Contains(2, items);
        }
    }
}