// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Distinct
{
    using System.Threading;
    using System.Threading.Tasks;
    using Serpent.MessageBus.Interfaces;

    using Xunit;

    public class DistinctWireUpTests
    {
        [Fact]
        public async Task DistinctWireUp_Attribute_Test()
        {
            var bus = new ConcurrentMessageBus<Message>();

            var handler = new DistinctMessageHandler();

            bus.Subscribe(b => b.WireUp(handler));

            await bus.PublishAsync(
                new Message
                    {
                        Id = "1"
                    });

            await bus.PublishAsync(
                new Message
                    {
                        Id = "1"
                    });

            await bus.PublishAsync(
                new Message
                    {
                        Id = "1"
                    });

            await bus.PublishAsync(
                new Message
                    {
                        Id = "2"
                    });

            Assert.Equal(2, handler.NumberOfInvokations);

            bus.Subscribe(b => b.WireUp(handler));
            bus.Subscribe(b => b.WireUp(handler));
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

        private class Message
        {
            public string Id { get; set; }
        }
    }
}