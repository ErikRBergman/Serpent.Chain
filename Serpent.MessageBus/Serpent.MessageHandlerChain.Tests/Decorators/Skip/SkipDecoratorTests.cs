// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.Skip
{
    using Xunit;

    public class SkipDecoratorTests
    {
        [Fact]
        public void SKipDecorator_Tests()
        {
            var count = 0;

            var func = Create.SimpleFunc<int>(s => s.Skip(2).Handler(m => count++));

            Assert.Equal(0, count);

            func(1);
            func(2);

            // 2 should be skipped
            Assert.Equal(0, count);

            func(3);
            func(4);

            Assert.Equal(2, count);
        }
    }
}