namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.LimitedThroughput
{
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class LimitedThroughputWireUp : BaseWireUp<LimitedThroughputAttribute>
    {
        public override void WireUp<TMessageType, THandlerType>(
            LimitedThroughputAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.LimitedThroughput(attribute.MaxNumberOfMessagesPerPeriod, attribute.Period);
        }
    }
}