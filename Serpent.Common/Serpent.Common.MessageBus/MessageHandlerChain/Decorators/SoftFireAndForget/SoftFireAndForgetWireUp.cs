namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SoftFireAndForget
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class SoftFireAndForgetWireUp : BaseWireUp<SoftFireAndForgetAttribute, SoftFireAndForgetConfiguration>
    {
        protected override SoftFireAndForgetConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new SoftFireAndForgetConfiguration();
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(SoftFireAndForgetAttribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.SoftFireAndForget();
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            SoftFireAndForgetConfiguration configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.SoftFireAndForget();
        }
    }
}