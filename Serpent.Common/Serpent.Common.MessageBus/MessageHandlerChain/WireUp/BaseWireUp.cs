namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;

    public abstract class BaseWireUp<TAttributeType> : IWireUp
        where TAttributeType : WireUpAttribute
    {
        public Type AttributeType { get; } = typeof(TAttributeType);

        public void WireUp<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (attribute is TAttributeType ourAttribute)
            {
                this.WireUp(ourAttribute, messageHandlerChainBuilder, handler);
            }
        }

        public abstract void WireUp<TMessageType, THandlerType>(
            TAttributeType attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler);
    }
}