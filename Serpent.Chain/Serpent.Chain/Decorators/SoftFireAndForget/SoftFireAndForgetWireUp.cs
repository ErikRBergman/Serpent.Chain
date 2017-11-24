namespace Serpent.Chain.Decorators.SoftFireAndForget
{
    using Serpent.Chain.WireUp;

    internal class SoftFireAndForgetWireUp : BaseWireUp<SoftFireAndForgetAttribute, SoftFireAndForgetConfiguration>
    {
        protected override SoftFireAndForgetConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new SoftFireAndForgetConfiguration();
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(SoftFireAndForgetAttribute attribute, IChainBuilder<TMessageType> chainBuilder, THandlerType handler)
        {
            chainBuilder.SoftFireAndForget();
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            SoftFireAndForgetConfiguration configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            chainBuilder.SoftFireAndForget();
        }
    }
}