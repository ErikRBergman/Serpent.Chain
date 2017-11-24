namespace Serpent.Chain.Decorators.NoDuplicates
{
    using System.Linq;
    using System.Reflection;

    using Serpent.Chain.WireUp;

    internal class NoDuplicatesWireUp : BaseWireUp<NoDuplicatesAttribute, NoDuplicatesConfiguration>
    {
#pragma warning disable CC0021 // Use nameof
        internal const string WireUpExtensionName = "NoDuplicatesWireUp";
#pragma warning restore CC0021 // Use nameof

        protected override NoDuplicatesConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new NoDuplicatesConfiguration
            {
                PropertyName = text
            };
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            NoDuplicatesAttribute attribute,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            var propertyName = attribute.PropertyName;

            WireUp<TMessageType>(chainBuilder, propertyName);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            NoDuplicatesConfiguration configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            WireUp<TMessageType>(chainBuilder, configuration.PropertyName);
        }

        private static void WireUp<TMessageType>(IChainBuilder<TMessageType> chainBuilder, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                chainBuilder.NoDuplicates(msg => msg.ToString());
                return;
            }

            SelectorSetup<TMessageType>.WireUp(
                propertyName,
                    typeof(NoDuplicatesExtensions).GetMethods()
                    .FirstOrDefault(
                        m => m.IsGenericMethodDefinition && m.IsStatic && m.GetCustomAttributes<ExtensionMethodSelectorAttribute>().Any(a => a.Identifier == WireUpExtensionName)),
                (methodInfo, selector) => methodInfo.Invoke(null, new object[] { chainBuilder, selector }));
        }
    }
}