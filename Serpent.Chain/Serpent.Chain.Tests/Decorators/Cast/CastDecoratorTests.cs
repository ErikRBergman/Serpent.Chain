// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Cast
{
    using Xunit;

    public class CastDecoratorTests
    {
        [Fact]
        public void CastDecorator_Test()
        {
            var result = 0;

            var func = Create.SimpleFunc<T2>(b => b.Cast<T2, T1>().Handler(m => result = m.BaseProp));

            func(
                new T2
                    {
                        BaseProp = 5
                    });

            Assert.Equal(5, result);
        }

        private class T1
        {
            public int BaseProp { get; set; }
        }

        private class T2 : T1
        {
        }
    }
}