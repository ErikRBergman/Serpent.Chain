namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SoftFireAndForget
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class SoftFireAndForgetWireUp : IWireUp
    {
        public Type AttributeType { get; } = typeof(WireUpAttribute);

        public void WireUp<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (attribute is SoftFireAndForgetAttribute)
            {
                messageHandlerChainBuilder.SoftFireAndForget();
            }
        }
    }
}