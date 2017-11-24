// ReSharper disable StyleCop.SA1126 - invalid warning
namespace Serpent.Chain.Decorators.Concurrent
{
    using System;

    using Serpent.Chain.WireUp;

    internal class ConcurrentWireUp : BaseWireUp<ConcurrentAttribute, ConcurrentConfiguration>
    {
        protected override ConcurrentConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            if (int.TryParse(text, out var concurrencyLevel))
            {
                return new ConcurrentConfiguration
                           {
                               MaxNumberOfConcurrentMessages = concurrencyLevel
                           };
            }

            throw new Exception("Concurrent: Could not convert concurrency level to integer: " + text);
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(ConcurrentAttribute attribute, IChainBuilder<TMessageType> chainBuilder, THandlerType handler)
        {
            chainBuilder.Concurrent(attribute.MaxNumberOfConcurrentMessages);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(ConcurrentConfiguration configuration, IChainBuilder<TMessageType> chainBuilder, THandlerType handler)
        {
            chainBuilder.Concurrent(configuration.MaxNumberOfConcurrentMessages);
        }
    }
}