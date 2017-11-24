// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Distinct
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Distinct;

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

        [Fact]
        public async Task DistinctWireUp_Configuration_Test()
        {
            var wireUp = new DistinctWireUp();
            var propertyName = nameof(DistinctTestMessage.Id);

            var config = wireUp.CreateConfigurationFromDefaultValue(propertyName);

            var count = 0;

            var bus = Create.SimpleFunc<DistinctTestMessage>(
                s => s
                    .WireUp((IEnumerable<object>)new[] { config })
                    .Handler(_ => count++));

            await bus(new DistinctTestMessage("1"));
            await bus(new DistinctTestMessage("1"));
            await bus(new DistinctTestMessage("1"));
            await bus(new DistinctTestMessage("2"));

            Assert.Equal(2, count);
        }
    }
}