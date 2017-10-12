namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    [WireUpConfigurationName("Dinstinct")]
    public class DistinctConfiguration
    {
        public string PropertyName { get; set; }
    }
}