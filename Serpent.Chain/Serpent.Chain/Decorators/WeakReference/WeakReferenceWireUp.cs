// ReSharper disable StyleCop.SA1126
namespace Serpent.Chain.Decorators.WeakReference
{
    using System;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.WireUp;

    internal class WeakReferenceWireUp : BaseWireUp<WeakReferenceAttribute, WeakReferenceConfiguration>
    {
        protected override WeakReferenceConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            return new WeakReferenceConfiguration();
        }

        protected override bool WireUpFromAttribute<TMessageType, THandlerType>(
            WeakReferenceAttribute attribute,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            if (handler is IMessageHandler<TMessageType> messageHandler)
            {
                chainBuilder.WeakReference(messageHandler);
            }
            else
            {
                throw new Exception("The handler must be of type IMessageHandler<TMessageType> to use WeakReference");
            }

            return false;
        }

        protected override bool WireUpFromConfiguration<TMessageType, THandlerType>(
            WeakReferenceConfiguration configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            if (handler is IMessageHandler<TMessageType> messageHandler)
            {
                chainBuilder.WeakReference(messageHandler);
            }
            else
            {
                throw new Exception("The handler must be of type IMessageHandler<TMessageType> to use WeakReference");
            }

            return false;
        }
    }
}