namespace Serpent.MessageHandlerChain.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Exceptions;

    using Xunit;

    public class MessageHandlerChainBuilderTests
    {
        [Fact]
        public void AddDecoratorAfterHandlerTest()
        {
            Assert.Throws<MessageHandlerChainHasAHandlerException>(() => Create.SimpleFunc<int>(b => b.SoftFireAndForget().Handler(m => { }).SoftFireAndForget()));
        }

        [Fact]
        public void NoHandlerException()
        {
            Assert.Throws<MessageHandlerChainHasNoMessageHandlerException>(() => Create.SimpleFunc<int>(b => b.SoftFireAndForget()));
        }
        
        [Fact]
        public async Task SubscriptionBuilderStackTests()
        {
            var func = Create.SimpleFunc<Message>(
                b => b.Action(
                        c => c.Before(message => message.Steps.Add("before1"))
                            .Finally(
                                message =>
                                    {
                                        if (message.Steps.Contains("after2"))
                                        {
                                            message.Steps.Add("after1");
                                        }
                                    }))
                    .Action(
                        c => c.Before(
                                message =>
                                    {
                                        if (message.Steps.Contains("before1"))
                                        {
                                            message.Steps.Add("before2");
                                        }
                                    })
                            .Finally(message => message.Steps.Add("after2")))
                    .Handler(
                        message =>
                            {
                                message.Steps.Add("handler");
                                message.HandlerInvoked = "yes";
                            }));

            var msg = new Message();
            await func(msg);

            Assert.Equal("yes", msg.HandlerInvoked);

            Assert.Equal("before1", msg.Steps[0]);
            Assert.Equal("before2", msg.Steps[1]);

            Assert.Equal("handler", msg.Steps[2]);

            Assert.Equal("after2", msg.Steps[3]);
            Assert.Equal("after1", msg.Steps[4]);
        }

        private class Message
        {
            public string HandlerInvoked { get; set; }

            public string Id { get; set; }

            public List<string> Steps { get; } = new List<string>();
        }

        private class OuterMessage
        {
            public Message Message { get; set; }

            public CancellationToken Token { get; set; }
        }
    }
}