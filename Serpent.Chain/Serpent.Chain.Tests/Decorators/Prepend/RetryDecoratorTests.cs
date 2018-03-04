namespace Serpent.Chain.Tests.Decorators.Prepend
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Exceptions;
    using Serpent.Chain.Helpers;
    using Serpent.Chain.Models;

    using Xunit;

    public class PrependDecoratorTests
    {
        [Fact]
        public async Task PrependTest()
        {
            var result = new List<int>();

            var func = Create.SimpleFunc<int>(
                b => b
                    .Prepend(msg => msg + 1).Prepend(msg => msg + 1)
                    .Handler(message => result.Add(message)));

            await func(5);

            Assert.Equal(4, result.Count);

            Assert.Equal(7, result[0]);
            Assert.Equal(6, result[1]);
            Assert.Equal(6, result[2]);
            Assert.Equal(5, result[3]);

        }
    }
}