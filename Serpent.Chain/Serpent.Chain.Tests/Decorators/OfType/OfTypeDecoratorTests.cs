namespace Serpent.Chain.Tests.Decorators.OfType
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.Chain;

    using Xunit;

    public class MessageBusExtensionsTests
    {
        [Fact]
        public void TestSingleBusMultipleTypesExtensions()
        {
            var type1Received = new List<MessageType1>();
            var type2Received = new List<MessageType2>();

            var messageType1Func = Create.SimpleFunc<BaseMessageType>(b => b.OfType<BaseMessageType, MessageType1>().Handler(
                msg =>
                    {
                        type1Received.Add(msg);
                        return Task.CompletedTask;
                    }));

            var messageType2Func = Create.SimpleFunc<BaseMessageType>(b => b.OfType<BaseMessageType, MessageType2>().Handler(
                msg =>
                    {
                        type2Received.Add(msg);
                        return Task.CompletedTask;
                    }));

            messageType1Func(new MessageType1("Haj"));
            messageType2Func(new MessageType2 { Name = "Boj" });

            Assert.Single(type1Received);
            Assert.Single(type2Received);
        }

        private class BaseMessageType
        {
        }

        // Messages should be immutable like this one
        private class MessageType1 : BaseMessageType
        {
            public MessageType1(string name)
            {
                this.Name = name;
            }

            public string Name { get; }
        }

        private class MessageType2 : BaseMessageType
        {
            public string Name { get; set; }
        }
    }
}