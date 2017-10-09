namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class DistinctAttribute : WireUpAttribute
    {
        public DistinctAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}