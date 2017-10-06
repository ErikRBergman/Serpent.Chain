namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Delay
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class DelayWireUp : IWireUp
    {
        public Type AttributeType { get; } = typeof(DelayAttribute);

        public void WireUp<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (attribute is DelayAttribute delayAttribute)
            {
                messageHandlerChainBuilder.Delay(delayAttribute.Delay);
            }
        }
    }
}