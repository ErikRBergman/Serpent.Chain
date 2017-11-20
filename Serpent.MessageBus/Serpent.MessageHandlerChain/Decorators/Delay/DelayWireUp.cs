// ReSharper disable StyleCop.SA1126
namespace Serpent.MessageHandlerChain.Decorators.Delay
{
    using System;

    using Serpent.MessageHandlerChain.Exceptions;
    using Serpent.MessageHandlerChain.WireUp;

    internal class DelayWireUp : BaseWireUp<DelayAttribute, DelayConfiguration>
    {
        protected override DelayConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            if (TimeSpan.TryParse(text, out var delay))
            {
                return new DelayConfiguration
                           {
                               Delay = delay
                           };
            }

            throw new CouldNotParseConfigTextToTimeSpanException("Delay: Could not parse text to timespan: " + text, text);
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            DelayAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.Delay(attribute.Delay);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            DelayConfiguration configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            messageHandlerChainBuilder.Delay(configuration.Delay);
        }
    }
}