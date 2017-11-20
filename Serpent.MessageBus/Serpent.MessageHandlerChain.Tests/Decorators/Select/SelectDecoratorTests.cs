// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.Select
{
    using System.Threading.Tasks;

    using Xunit;

    public class SelectDecoratorTests
    {
        [Fact]
        public async Task SelectDecorator_Test()
        {
            var func = Create.SimpleFunc<Message>(b => b
                .Select(
                    message => new OuterMessage
                                   {
                                       Message = message,
                                       Context = "Selector"
                                   })
                .Action(c => c.Before(msg => msg.Message.Step1 = true))
                .Select(message => message.Message)
                .Handler(message => message.Step2 = true));

            var messageToPublish = new Message();

            await func(messageToPublish);

            Assert.True(messageToPublish.Step1);
            Assert.True(messageToPublish.Step2);
            Assert.False(messageToPublish.Step3);
        }

        private class Message
        {
            public bool Step1 { get; set; }

            public bool Step2 { get; set; }

            public bool Step3 { get; set; }
        }

        private class OuterMessage
        {
            public string Context { get; set; }

            public Message Message { get; set; }
        }
    }
}