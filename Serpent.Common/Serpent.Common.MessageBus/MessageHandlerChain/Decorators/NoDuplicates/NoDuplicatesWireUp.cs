namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.NoDuplicates
{
    using System.Linq;
    using System.Reflection;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class NoDuplicatesWireUp : BaseWireUp<NoDuplicatesAttribute>
    {
        public const string WireUpExtensionName = "NoDuplicatesWireUp";

        public override void WireUp<TMessageType, THandlerType>(
            NoDuplicatesAttribute attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            SelectorSetup<TMessageType, NoDuplicatesWireUp>
                .WireUp(
                    attribute.PropertyName,
                    () =>
                        typeof(DistinctExtensions)
                            .GetMethods()
                            .FirstOrDefault(
                                m => m.IsGenericMethodDefinition && m.IsStatic && m.GetCustomAttributes<ExtensionMethodSelectorAttribute>().Any(a => a.Identifier == WireUpExtensionName)),
                    (methodInfo, selector) => methodInfo.Invoke(null, new object[] { messageHandlerChainBuilder, selector }));
        }
    }
}