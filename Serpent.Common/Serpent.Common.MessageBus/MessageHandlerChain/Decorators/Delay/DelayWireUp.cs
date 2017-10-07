namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Delay
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class DelayWireUp : BaseWireUp<DelayAttribute>
    {
        public override void WireUp<TMessageType, THandlerType>(DelayAttribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Delay(attribute.Delay);
        }
    }
}