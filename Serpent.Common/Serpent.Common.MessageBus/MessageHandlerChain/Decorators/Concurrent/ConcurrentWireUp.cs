namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class ConcurrentWireUp : IWireUp
    {
        public Type AttributeType { get; } = typeof(ConcurrentAttribute);

        public void WireUp<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (attribute is ConcurrentAttribute concurrentAttribute)
            {
                messageHandlerChainBuilder.Concurrent(concurrentAttribute.MaxNumberOfConcurrentMessages);
            }
        }
    }
}