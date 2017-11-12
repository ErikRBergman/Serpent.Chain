// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Select
{
    using System.Threading.Tasks;

    using Xunit;

    public class SelectDecoratorTests
    {
        [Fact]
        public async Task SelectDecorator_Test()
        {
            var bus = new ConcurrentMessageBus<Message>();

            bus.Subscribe(b => b
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

            await bus.PublishAsync(messageToPublish);

            Assert.True(messageToPublish.Step1);
            Assert.True(messageToPublish.Step2);
            Assert.False(messageToPublish.Step3);
        }

        private class Message
        {
            public string HandlerInvoked { get; set; }

            public string Id { get; set; }

            public bool Step1 { get; set; }

            public bool Step2 { get; set; }

            public bool Step3 { get; set; }

            public bool Step4 { get; set; }

            public bool Step5 { get; set; }

            public bool Step6 { get; set; }
        }

        private class OuterMessage
        {
            public string Context { get; set; }

            public Message Message { get; set; }
        }
    }
}