namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.FireAndForget
{
    using System.Threading.Tasks;

    using Xunit;

    public class FireAndForgetDecoratorTests
    {
        [Fact]
        public async Task TestFireAndForgetSubscription()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            bus.Subscribe(b => b.FireAndForget()
                .Handler(
                async msgz =>
                    {
                        await Task.Delay(100);
                        msgz.Status = "Got it!";
                    }));

            var msg = new Message1();

            await bus.PublishAsync(msg);
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