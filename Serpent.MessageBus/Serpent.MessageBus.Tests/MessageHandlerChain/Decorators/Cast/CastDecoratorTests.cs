using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Cast
{
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class CastDecoratorTests
    {
        [Fact]
        public void CastDecorator_Test()
        {
            var result = 0;

            var func = Create.Func<T2>(b => b.Cast<T2, T1>().Handler(m => result = m.BaseProp));

            func(
                new T2
                    {
                        BaseProp = 5
                    },
                CancellationToken.None);

            Assert.Equal(5, result);
        }


        private class T1
        {
            public int BaseProp { get; set; }
        }

        private class T2 : T1
        {
            public int OuterProp { get; set; }
        }
    }
}
