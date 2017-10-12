namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Delay
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class DelayWireUp : BaseWireUp<DelayAttribute, DelayConfiguration>
    {
        protected override void WireUpFromAttribute<TMessageType, THandlerType>(DelayAttribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Delay(attribute.Delay);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(DelayConfiguration configuration, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Delay(configuration.Delay);
        }
    }
}