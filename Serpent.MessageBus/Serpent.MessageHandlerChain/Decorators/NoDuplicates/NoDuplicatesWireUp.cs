namespace Serpent.MessageHandlerChain.Decorators.NoDuplicates
{
    using System.Linq;
    using System.Reflection;

    using Serpent.MessageHandlerChain.WireUp;

    internal class NoDuplicatesWireUp : BaseWireUp<NoDuplicatesAttribute, NoDuplicatesConfiguration>
    {
        internal const string WireUpExtensionName = "NoDuplicatesWireUp";

        protected override NoDuplicatesConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new NoDuplicatesConfiguration
                       {
                           PropertyName = text
                       };
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            NoDuplicatesAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            var propertyName = attribute.PropertyName;

            WireUp<TMessageType>(messageHandlerChainBuilder, propertyName);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            NoDuplicatesConfiguration configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            WireUp<TMessageType>(messageHandlerChainBuilder, configuration.PropertyName);
        }

        private static void WireUp<TMessageType>(IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                messageHandlerChainBuilder.NoDuplicates(msg => msg.ToString());
                return;
            }

            SelectorSetup<TMessageType>.WireUp(
                propertyName,
                    typeof(NoDuplicatesExtensions).GetMethods()
                    .FirstOrDefault(
                        m => m.IsGenericMethodDefinition && m.IsStatic && m.GetCustomAttributes<ExtensionMethodSelectorAttribute>().Any(a => a.Identifier == WireUpExtensionName)),
                (methodInfo, selector) => methodInfo.Invoke(null, new object[] { messageHandlerChainBuilder, selector }));
        }
    }
}