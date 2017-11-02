namespace Serpent.MessageBus.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConcurrentMessageBusTests
    {
        [TestMethod]
        public async Task TestConcurrentMessageBusNormal()
        {
            var messageType1Bus = new ConcurrentMessageBus<MessageType1>();

            var type1Received = new List<MessageType1>();

            messageType1Bus.Subscribe(
                message =>
                    {
                        type1Received.Add(message);
                        return Task.CompletedTask;
                    });

            const string Text = "Test";

            await messageType1Bus.PublishAsync(new MessageType1(Text));
            Assert.AreEqual(1, type1Received.Count);

            Assert.AreEqual(Text, type1Received.First().Name);
        }

        [TestMethod]
        public async Task TestConcurrentMessageBusPatternMatching()
        {
            // If you would rather have a single message bus for your entire application, that can be done in this way
            var bus = new ConcurrentMessageBus<MessageBase>();

            var type1Received = new List<MessageType1>();
            var type2Received = new List<MessageType2>();

            bus.Subscribe(
                message =>
                    {
                        switch (message)
                        {
                            case MessageType1 msg:
                                type1Received.Add(msg);
                                break;

                            case MessageType2 msg:
                                type2Received.Add(msg);
                                break;
                        }

                        return Task.CompletedTask;
                    });

            await bus.PublishAsync(new MessageType1("John Yolo"));

            Assert.IsTrue(type1Received.Count == 1);
            Assert.IsTrue(type2Received.Count == 0);

            await bus.PublishAsync(new MessageType2("John Yolo"));

            Assert.IsTrue(type1Received.Count == 1);
            Assert.IsTrue(type2Received.Count == 1);
        }

        private class MessageBase
        {
        }

        // Messages should be immutable like this one
        private class MessageType1 : MessageBase
        {
            public MessageType1(string name)
            {
                this.Name = name;
            }

            public string Name { get; }
        }

        private class MessageType2 : MessageBase
        {
            public MessageType2(string name)
            {
                this.Name = name;
            }

            public string Name { get; set; }
        }
    }
}