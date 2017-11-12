namespace Serpent.MessageBus.Tests
{
    public class MessageBusExtensionsTests
    {
        ////[Fact]
        ////public void TestSingleBusMultipleTypesExtensions()
        ////{
        ////    var bus = new ConcurrentMessageBus<BaseMessageType>();

        ////    var type1Received = new List<MessageType1>();
        ////    var type2Received = new List<MessageType2>();

        ////    var subscription1 = bus.Subscribe<BaseMessageType, MessageType1>(
        ////        msg =>
        ////            {
        ////                type1Received.Add(msg);
        ////                return Task.CompletedTask;
        ////            });

        ////    var subscription2 = bus.Subscribe<BaseMessageType, MessageType2>(
        ////        msg =>
        ////            {
        ////                type2Received.Add(msg);
        ////                return Task.CompletedTask;
        ////            });

        ////    bus.PublishAsync(new MessageType1("Haj"));
        ////    bus.PublishAsync(
        ////        new MessageType2()
        ////            {
        ////                Name = "Boj"
        ////            });

        ////    Assert.Equal(1, type1Received.Count);
        ////    Assert.Equal(1, type2Received.Count);

        ////    subscription2.Dispose();
        ////    subscription1.Dispose();
        ////}

        private class BaseMessageType
        {
            public int Id { get; set; }
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