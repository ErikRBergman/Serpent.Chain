// ReSharper disable StyleCop.SA1126
namespace Serpent.MessageHandlerChain.Decorators.WeakReference
{
    using Serpent.MessageHandlerChain.WireUp;

    internal class WeakReferenceWireUp : BaseWireUp<WeakReferenceAttribute, WeakReferenceConfiguration>
    {
        protected override WeakReferenceConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new WeakReferenceConfiguration();
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            WeakReferenceAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.WeakReference();
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            WeakReferenceConfiguration configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.WeakReference();
        }
    }
}