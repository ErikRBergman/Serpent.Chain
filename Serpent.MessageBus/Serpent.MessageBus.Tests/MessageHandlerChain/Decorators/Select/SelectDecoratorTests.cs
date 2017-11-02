// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain
{
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SelectDecoratorTests
    {
        [TestMethod]
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
                .Filter(msg => msg.Message.Step1 = true)
                .Select(message => message.Message)
                .Handler(message => message.Step2 = true));

            var messageToPublish = new Message();

            await bus.PublishAsync(messageToPublish);

            Assert.IsTrue(messageToPublish.Step1);
            Assert.IsTrue(messageToPublish.Step2);
            Assert.IsFalse(messageToPublish.Step3);
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