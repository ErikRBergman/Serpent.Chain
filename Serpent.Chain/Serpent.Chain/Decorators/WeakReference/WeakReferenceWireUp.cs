// ReSharper disable StyleCop.SA1126
namespace Serpent.Chain.Decorators.WeakReference
{
    using Serpent.Chain.WireUp;

    internal class WeakReferenceWireUp : BaseWireUp<WeakReferenceAttribute, WeakReferenceConfiguration>
    {
        protected override WeakReferenceConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new WeakReferenceConfiguration();
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            WeakReferenceAttribute attribute,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            chainBuilder.WeakReference();
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            WeakReferenceConfiguration configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            chainBuilder.WeakReference();
        }
    }
}