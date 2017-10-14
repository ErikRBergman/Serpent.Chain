namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct
{
    using System.Linq;
    using System.Reflection;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    internal class DistinctWireUp : BaseWireUp<DistinctAttribute, DistinctConfiguration>
    {
        internal const string WireUpExtensionName = "DistinctWireUp";

        protected override DistinctConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new DistinctConfiguration();
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            DistinctAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            if (string.IsNullOrWhiteSpace(attribute.PropertyName))
            {
                messageHandlerChainBuilder.Distinct(msg => false);
                return;
            }

            SelectorSetup<TMessageType, DistinctWireUp>
                .WireUp(
                    attribute.PropertyName,
                    () =>
                        typeof(DistinctExtensions)
                            .GetMethods()
                            .FirstOrDefault(
                                m => m.IsGenericMethodDefinition && m.IsStatic && m.GetCustomAttributes<ExtensionMethodSelectorAttribute>().Any(a => a.Identifier == WireUpExtensionName)),
                    (methodInfo, selector) => methodInfo.Invoke(null, new object[] { messageHandlerChainBuilder, selector }));
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(DistinctConfiguration configuration, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (string.IsNullOrWhiteSpace(configuration.PropertyName))
            {
                messageHandlerChainBuilder.Distinct(msg => msg.ToString());
                return;
            }

            SelectorSetup<TMessageType, DistinctWireUp>
                .WireUp(
                    configuration.PropertyName,
                    () =>
                        typeof(DistinctExtensions)
                            .GetMethods()
                            .FirstOrDefault(
                                m => m.IsGenericMethodDefinition && m.IsStatic && m.GetCustomAttributes<ExtensionMethodSelectorAttribute>().Any(a => a.Identifier == WireUpExtensionName)),
                    (methodInfo, selector) => methodInfo.Invoke(null, new object[] { messageHandlerChainBuilder, selector }));
        }
    }
}