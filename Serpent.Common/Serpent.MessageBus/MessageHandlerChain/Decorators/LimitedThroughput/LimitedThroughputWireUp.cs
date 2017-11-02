// ReSharper disable StyleCop.SA1126
namespace Serpent.MessageBus.MessageHandlerChain.Decorators.LimitedThroughput
{
    using System;

    using Serpent.MessageBus.MessageHandlerChain.WireUp;

    internal class LimitedThroughputWireUp : BaseWireUp<LimitedThroughputAttribute, LimitedThroughputConfiguration>
    {
        protected override LimitedThroughputConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            if (int.TryParse(text, out var maxNumberOfMessagesPerPeriod))
            {
                return new LimitedThroughputConfiguration
                           {
                               MaxNumberOfMessagesPerPeriod = maxNumberOfMessagesPerPeriod,
                               Period = TimeSpan.FromSeconds(1)
                           };
            }

            throw new Exception("LimitedThroughput: Could not convert text to integer " + text);
        }

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