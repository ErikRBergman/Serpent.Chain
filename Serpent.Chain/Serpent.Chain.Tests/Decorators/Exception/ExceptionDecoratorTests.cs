// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Exception
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Helpers;

    using Xunit;

    public class ExceptionDecoratorTests
    {
        [Fact]
        public async Task Test_Exception_NoPropagation_Decorator()
        {
            var func = Create.SimpleFunc<string>(
                b => b.Exception(
                        (msg, exception, token) =>
                            {
                                Assert.Equal(msg, exception.Message);
                                return TaskHelper.FalseTask;
                            })
                    .Handler(msg => throw new Exception(msg)));

            await func("ABC");
        }

        [Fact]
        public async Task Test_ExceptionDecorator()
        {
            var func = Create.SimpleFunc<string>(
                b => b.Exception(
                        (msg, exception, token) =>
                            {
                                Assert.Equal(msg, exception.Message);
                                return TaskHelper.TrueTask;
                            })
                    .Handler(msg => throw new Exception(msg)));

            var ex = await Assert.ThrowsAsync<Exception>(async () => await func("ABC"));
            Assert.Equal("ABC", ex.Message);
        }
    }
}