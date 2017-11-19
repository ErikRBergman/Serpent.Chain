// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Distinct
{
    using System.Threading.Tasks;

    using Xunit;

    public class DistinctWireUpTests
    {
        [Fact]
        public async Task DistinctWireUp_Attribute_Test()
        {
            var handler = new DistinctMessageHandler();

            var bus = Create.SimpleFunc<DistinctTestMessage>(s => s.WireUp(handler));

            await bus(new DistinctTestMessage("1"));
            await bus(new DistinctTestMessage("1"));
            await bus(new DistinctTestMessage("1"));
            await bus(new DistinctTestMessage("2"));

            Assert.Equal(2, handler.NumberOfInvokations);
        }

        [Fact]
        public async Task DistinctWireUp_ValueType_AttributeNoParameters_Test()
        {
            var handler = new DistinctValueTypeMessageHandler();

            var func = Create.SimpleFunc<int>(s => s.WireUp(handler));

            await func(1);
            await func(1);
            await func(1);
            await func(1);
            await func(1);
            await func(2);

            Assert.Equal(2, handler.NumberOfInvokations);
        }
    }
}