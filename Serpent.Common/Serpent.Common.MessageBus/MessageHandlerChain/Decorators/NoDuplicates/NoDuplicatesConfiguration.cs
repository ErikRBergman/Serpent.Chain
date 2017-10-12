namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.NoDuplicates
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    [WireUpConfigurationName("NoDuplicates")]
    public class NoDuplicatesConfiguration
    {
        public string PropertyName { get; }
    }
}