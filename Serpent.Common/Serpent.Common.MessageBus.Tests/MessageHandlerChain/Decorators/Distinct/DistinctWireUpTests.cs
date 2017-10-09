// ReSharper disable InconsistentNaming

namespace Serpent.Common.MessageBus.Tests.MessageHandlerChain.Decorators.Distinct
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct;

    [TestClass]
    public class DistinctWireUpTests
    {
        [TestMethod]
        public async Task DistinctWireUp_Attribute_Test()
        {
            var bus = new ConcurrentMessageBus<Message>();

            var handler = new DistinctMessageHandler();

            bus.Subscribe().WireUp(handler);

            await bus.PublishAsync(new Message()
                                       {
                                           Id = "1"
                                       });

            await bus.PublishAsync(new Message()
                                       {
                                           Id = "1"
                                       });

            await bus.PublishAsync(new Message()
                                       {
                                           Id = "1"
                                       });

            await bus.PublishAsync(new Message()
                                       {
                                           Id = "2"
                                       });


            Assert.AreEqual(2, handler.NumberOfInvokations);

            bus.Subscribe().WireUp(handler);
            bus.Subscribe().WireUp(handler);

        }

        private class Message
        {
            public string Id { get; set; }
        }

        [Distinct(nameof(Message.Id))]
        private class DistinctMessageHandler : IMessageHandler<Message>
        {
            public int NumberOfInvokations { get; set; }

            public Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
            {
                this.NumberOfInvokations++;

                return Task.CompletedTask;
            }
        }

    }
}