namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;

    public abstract class BaseWireUp<TAttributeType, TConfigurationType> : IWireUp
        where TAttributeType : WireUpAttribute
    {
        /// <summary>
        /// Type of the attribute used for attribute wire up
        /// </summary>
        public Type AttributeType { get; } = typeof(TAttributeType);

        /// <summary>
        /// Type of the configuration object used for configuration wire up
        /// </summary>
        public Type ConfigurationType { get; } = typeof(TConfigurationType);

        /// <summary>
        /// Wire up from attribute
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <typeparam name="THandlerType"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="messageHandlerChainBuilder"></param>
        /// <param name="handler"></param>
        public void WireUpFromAttribute<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (attribute is TAttributeType ourAttribute)
            {
                this.WireUpFromAttribute(ourAttribute, messageHandlerChainBuilder, handler);
            }
        }

        /// <summary>
        /// Wire up from configuration
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <typeparam name="THandlerType"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="messageHandlerChainBuilder"></param>
        /// <param name="handler"></param>
        public void WireUpFromConfiguration<TMessageType, THandlerType>(
            object configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler)
        {
            if (configuration is TConfigurationType outConfiguration)
            {
                this.WireUpFromConfiguration(outConfiguration, messageHandlerChainBuilder, handler);
            }
        }

        protected abstract void WireUpFromAttribute<TMessageType, THandlerType>(
            TAttributeType attribute,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler);

        protected abstract void WireUpFromConfiguration<TMessageType, THandlerType>(
            TConfigurationType configuration,
            IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            THandlerType handler);
    }
}