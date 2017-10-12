namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.LimitedThroughput
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class LimitedThroughputWireUp : BaseWireUp<LimitedThroughputAttribute, LimitedThroughputConfiguration>
    {
        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            LimitedThroughputAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.LimitedThroughput(attribute.MaxNumberOfMessagesPerPeriod, attribute.Period);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            LimitedThroughputConfiguration configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.LimitedThroughput(configuration.MaxNumberOfMessagesPerPeriod, configuration.Period);
        }
    }
}