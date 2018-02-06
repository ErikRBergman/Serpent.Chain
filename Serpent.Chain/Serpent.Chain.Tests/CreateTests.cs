namespace Serpent.Chain.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class CreateTests
    {
        [Fact]
        public void CreateChainTest()
        {
            var count = 0;
            var chain = Create.CreateChain<int>(b => b.First().Handler(m => { count++; }));
            chain.ChainFunc(0, CancellationToken.None);
            Assert.Equal(1, count);
        }

        [Fact]
        public void CreateFuncTest()
        {
            var count = 0;
            var func = Create.Func<int>(b => b.First().Handler(m => { count++; }));
            func(0, CancellationToken.None);
            Assert.Equal(1, count);
        }

        [Fact]
        public void SimpleFuncTest()
        {
            var count = 0;
            var func = Create.SimpleFunc<int>(b => b.First().Handler(m => { count++; }));
            func(0);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task TestReturnValue()
        {
            var func = Create.SimpleRequestResponse<int, string>(b => b.HandlerWithResult(msg => msg.ToString()));
            Assert.Equal("5", await func(5));
        }
    }
}