namespace Serpent.MessageHandlerChain.Tests.Decorators.FireAndForget
{
    using System.Threading.Tasks;

    using Xunit;

    public class FireAndForgetDecoratorTests
    {
        [Fact]
        public async Task TestFireAndForgetSubscription()
        {
            var bus = Create.SimpleFunc<Message1>(b => b.FireAndForget()
                .Handler(
                async msgz =>
                    {
                        await Task.Delay(100);
                        msgz.Status = "Got it!";
                    }));

            var msg = new Message1();

            await bus(msg);
            Assert.NotEqual("Got it!", msg.Status);
            
            await Task.Delay(200);

            Assert.Equal("Got it!", msg.Status);
        }

        private class Message1
        {
            public string Status { get; set; }
        }
    }
}