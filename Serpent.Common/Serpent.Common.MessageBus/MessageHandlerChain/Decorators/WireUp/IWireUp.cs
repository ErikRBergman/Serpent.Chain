namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp
{
    using System;

    public interface IWireUp
    {
        Type AttributeType { get; }

        void WireUp<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler);
    }
}