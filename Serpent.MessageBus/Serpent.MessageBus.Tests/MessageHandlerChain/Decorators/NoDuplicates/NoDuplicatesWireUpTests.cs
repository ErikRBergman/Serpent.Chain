namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.NoDuplicates
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.NoDuplicates;

    using Xunit;

    public class NoDuplicatesWireUpTests
    {
        [Fact]
        public async Task TestWireUpFromConfiguration()
        {
            var wireUp = new NoDuplicatesWireUp();

            const string PropertyName = "Id";

            var config = wireUp.CreateConfigurationFromDefaultValue(PropertyName);

            var noDuplicatesConfiguration = config as NoDuplicatesConfiguration;
            Assert.NotNull(noDuplicatesConfiguration);

            Assert.Equal(PropertyName, noDuplicatesConfiguration.PropertyName);

            var handler = new Handler();

            var func = Create.Func<Message>(b => b.WireUp(handler, new[] { config }));

            Assert.Equal(0, handler.Count);

            var t1 = func(new Message(1), CancellationToken.None);
            t1 = func(new Message(1), CancellationToken.None);
            t1 = func(new Message(1), CancellationToken.None);
            t1 = func(new Message(1), CancellationToken.None);

            await Task.Delay(300);
            Assert.Equal(1, handler.Count);
        }

        [Fact]
        public async Task TestWireUpFromAttribute()
        {
            var handler = new Handler();

            var func = Create.Func<Message>(b => b.WireUp(handler));

            Assert.Equal(0, handler.Count);

            var t1 = func(new Message(1), CancellationToken.None);
            t1 = func(new Message(1), CancellationToken.None);
            t1 = func(new Message(1), CancellationToken.None);
            t1 = func(new Message(1), CancellationToken.None);

            await Task.Delay(300);
            Assert.Equal(1, handler.Count);
        }


        [NoDuplicates("Id")]
        private class Handler : IMessageHandler<Message>
        {
            public int Count { get; private set; }  

            public async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
            {
                this.Count++;
                await Task.Delay(500, cancellationToken);
            }
        }

        private class Message
        {
            public Message(int id)
            {
                this.Id = id;
            }

            public int Id { get; set; }
        }

    }
}