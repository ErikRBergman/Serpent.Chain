namespace Serpent.Chain.Tests.Decorators.ReturnValue
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class ReturnValueDecoratorTests
    {
        [Fact]
        public async Task HandlerWithResultTests()
        {
            var func = Create.RequestResponse<int, string>(b => b.HandlerWithResult((msg, token) => Task.FromResult(msg.ToString())));
            Assert.Equal("5", await func(5, CancellationToken.None));

            var errorFunc = Create.RequestResponse<int, string>(b => b.HandlerWithResult((msg, token) => throw new Exception(msg.ToString())));

            var exception = await Assert.ThrowsAsync<Exception>(
                async () => { await errorFunc(5, CancellationToken.None); });

            Assert.Equal("5", exception.Message);
        }
    }
}