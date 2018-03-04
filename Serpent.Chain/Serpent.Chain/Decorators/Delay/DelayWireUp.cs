// ReSharper disable StyleCop.SA1126
namespace Serpent.Chain.Decorators.Delay
{
    using System;

    using Serpent.Chain.Exceptions;
    using Serpent.Chain.WireUp;

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

        protected override bool WireUpFromAttribute<TMessageType, THandlerType>(
            DelayAttribute attribute,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            chainBuilder.Delay(attribute.Delay);
            return true;
        }

        protected override bool WireUpFromConfiguration<TMessageType, THandlerType>(
            DelayConfiguration configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            chainBuilder.Delay(configuration.Delay);
            return true;
        }
    }
}