// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Branch
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Xunit;

    public class BranchDecoratorTests
    {
        [Fact]
        public async Task BranchDecorator_Normal_Tests()
        {
            var items = new HashSet<int>();

            var func = Create.SimpleFunc<int>(b => b
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

            await func(1);

            Assert.Equal(2, items.Count);

            Assert.Contains(1, items);
            Assert.Contains(2, items);
        }
    }
}