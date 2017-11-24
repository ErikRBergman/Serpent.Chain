// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Select
{
    using System.Threading.Tasks;

    using Xunit;

    public class SelectAsyncDecoratorTests
    {
        [Fact]
        public async Task SelectDecorator_Test()
        {
            var func = Create.SimpleFunc<Message>(b => b
                .Select(
                    msg => Task.FromResult(new OuterMessage
                                   {
                                       Message = msg,
                                       Context = "Selector"
                                   }))
                .Action(c => c.Before(msg => msg.Message.Step1 = true))
                .Select(msg => msg.Message)
                .Handler(msg => msg.Step2 = true));

            var message = new Message();
            await func(message);

            Assert.True(message.Step1);
            Assert.True(message.Step2);
            Assert.False(message.Step3);
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