namespace Serpent.MessageBus.Tests.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class CreateTests
    {
        [Fact]
        public void CreateChainTest()
        {
            IMessageHandlerChain<int> chain = Create.CreateChain<int>(b => b.First().Handler(m => { }));
        }

        [Fact]
        public void CreateFuncTest()
        {
            Func<int, Task> func = Create.SimpleFunc<int>(b => b.First().Handler(m => { }));
        }

        [Fact]
        public void SimpleFuncTest()
        {
            Func<int, Task> func = Create.SimpleFunc<int>(b => b.First().Handler(m => { }));
        }

    }
}