namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class ConcurrentWireUp : BaseWireUp<ConcurrentAttribute>
    {
        public override void WireUp<TMessageType, THandlerType>(ConcurrentAttribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Concurrent(attribute.MaxNumberOfConcurrentMessages);
        }
    }
}