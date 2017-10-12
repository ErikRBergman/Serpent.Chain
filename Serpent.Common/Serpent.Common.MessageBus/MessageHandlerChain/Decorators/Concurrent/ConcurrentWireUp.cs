namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class ConcurrentWireUp : BaseWireUp<ConcurrentAttribute, ConcurrentConfiguration>
    {
        protected override void WireUpFromAttribute<TMessageType, THandlerType>(ConcurrentAttribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Concurrent(attribute.MaxNumberOfConcurrentMessages);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(ConcurrentConfiguration configuration, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Concurrent(configuration.MaxNumberOfConcurrentMessages);
        }
    }
}