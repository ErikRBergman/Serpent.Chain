namespace Serpent.Chain.Decorators.Distinct
{
    using System.Linq;
    using System.Reflection;

    using Serpent.Chain.WireUp;

    internal class DistinctWireUp : BaseWireUp<DistinctAttribute, DistinctConfiguration>
    {
#pragma warning disable CC0021 // Use nameof
        internal const string WireUpExtensionName = "DistinctWireUp";
#pragma warning restore CC0021 // Use nameof

        private static readonly MethodInfo DistinctMethodInfo = typeof(DistinctExtensions).GetMethods()
            .FirstOrDefault(
                m => m.IsGenericMethodDefinition && m.IsStatic && m.GetCustomAttributes<ExtensionMethodSelectorAttribute>().Any(a => a.Identifier == WireUpExtensionName));

        protected override DistinctConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new DistinctConfiguration
                       {
                           PropertyName = text
                       };
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            DistinctAttribute attribute,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            if (string.IsNullOrWhiteSpace(attribute.PropertyName))
            {
                chainBuilder.Distinct(msg => msg);
                return;
            }

            SelectorSetup<TMessageType>
                .WireUp(
                    attribute.PropertyName,
                        DistinctMethodInfo,
                    (typedMethodInfo, selector) => typedMethodInfo.Invoke(null, new object[] { chainBuilder, selector }));
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(DistinctConfiguration configuration, IChainBuilder<TMessageType> chainBuilder, THandlerType handler)
        {
            if (string.IsNullOrWhiteSpace(configuration.PropertyName))
            {
                chainBuilder.Distinct(msg => msg.ToString());
                return;
            }

            SelectorSetup<TMessageType>
                .WireUp(
                    configuration.PropertyName,
                    DistinctMethodInfo,
                    (methodInfo, selector) => methodInfo.Invoke(null, new object[] { chainBuilder, selector }));
        }
    }
}