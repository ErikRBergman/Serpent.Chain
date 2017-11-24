namespace Serpent.Chain.Tests.Decorators.NoDuplicates
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.NoDuplicates;
    using Serpent.Chain.Interfaces;

    using Xunit;

    public class NoDuplicatesWireUpTests
    {
        [Fact]
        public async Task TestWireUpFromAttribute()
        {
            var handler = new Handler();

            var func = Create.SimpleFunc<Message>(b => b.WireUp(handler));

            Assert.Equal(0, handler.Count);
            var m = new Func<int, Message>(v => new Message(v));

#pragma warning disable 4014
            func(m(1));
            func(m(1));
            func(m(1));
            func(m(1));
#pragma warning restore 4014

            await Task.Delay(300);
            Assert.Equal(1, handler.Count);
        }

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

            var func = Create.SimpleFunc<Message>(b => b.WireUp(handler, new[] { config }));

            Assert.Equal(0, handler.Count);

            var m = new Func<int, Message>(v => new Message(v));

#pragma warning disable 4014
            func(m(1));
            func(m(1));
            func(m(1));
            func(m(1));
#pragma warning restore 4014

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

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Id { get; }
        }
    }
}