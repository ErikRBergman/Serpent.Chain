namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.NoDuplicates
{
    using System.Linq;
    using System.Reflection;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class NoDuplicatesWireUp : BaseWireUp<NoDuplicatesAttribute, NoDuplicatesConfiguration>
    {
        internal const string WireUpExtensionName = "NoDuplicatesWireUp";

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(
            NoDuplicatesAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            var propertyName = attribute.PropertyName;

            WireUp<TMessageType, THandlerType>(messageHandlerChainBuilder, propertyName);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(
            NoDuplicatesConfiguration configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            WireUp<TMessageType, THandlerType>(messageHandlerChainBuilder, configuration.PropertyName);
        }

        private static void WireUp<TMessageType, THandlerType>(IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, string propertyName)
        {
            SelectorSetup<TMessageType, NoDuplicatesWireUp>.WireUp(
                propertyName,
                () => typeof(DistinctExtensions).GetMethods()
                    .FirstOrDefault(
                        m => m.IsGenericMethodDefinition && m.IsStatic && m.GetCustomAttributes<ExtensionMethodSelectorAttribute>().Any(a => a.Identifier == WireUpExtensionName)),
                (methodInfo, selector) => methodInfo.Invoke(null, new object[] { messageHandlerChainBuilder, selector }));
        }
    }
}