// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class NoDuplicatesAttribute : WireUpAttribute
    {
        public NoDuplicatesAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}