namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;

    public interface IWireUp
    {
        Type AttributeType { get; }

        Type ConfigurationType { get; }

        void WireUpFromAttribute<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler);

        void WireUpFromConfiguration<TMessageType, THandlerType>(object configuration, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler);
    }
}