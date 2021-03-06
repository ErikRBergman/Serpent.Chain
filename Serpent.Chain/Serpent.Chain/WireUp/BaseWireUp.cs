﻿namespace Serpent.Chain.WireUp
{
    using System;

    /// <summary>
    ///     The wire up base class
    /// </summary>
    /// <typeparam name="TAttributeType">The wire up attribute type</typeparam>
    /// <typeparam name="TConfigurationType">The wire up configuration type</typeparam>
    public abstract class BaseWireUp<TAttributeType, TConfigurationType> : IWireUp
        where TAttributeType : WireUpAttribute
    {
        /// <summary>
        ///     Type of the attribute used for attribute wire up
        /// </summary>
        public Type AttributeType { get; } = typeof(TAttributeType);

        /// <summary>
        ///     Type of the configuration object used for configuration wire up
        /// </summary>
        public Type ConfigurationType { get; } = typeof(TConfigurationType);

        /// <summary>
        ///     Create a decorator configuration from a default text value
        /// </summary>
        /// <param name="text">The configuration text</param>
        /// <returns>The decorator configuration</returns>
        public object CreateConfigurationFromDefaultValue(string text)
        {
            return this.CreateAndParseConfigurationFromDefaultValue(text);
        }

        /// <summary>
        ///     Wire up from attribute
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler type</typeparam>
        /// <param name="attribute">The attribute to wire up from</param>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="handler">The message handler</param>
        public bool WireUpFromAttribute<TMessageType, THandlerType>(Attribute attribute, IChainBuilder<TMessageType> chainBuilder, THandlerType handler)
        {
            if (attribute is TAttributeType ourAttribute)
            {
                return this.WireUpFromAttribute(ourAttribute, chainBuilder, handler);
            }

            return true;
        }

        /// <summary>
        ///     Wire up from configuration
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler type</typeparam>
        /// <param name="configuration">The configuration</param>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        public bool WireUpFromConfiguration<TMessageType, THandlerType>(
            object configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
        {
            if (configuration is TConfigurationType outConfiguration)
            {
                return this.WireUpFromConfiguration(outConfiguration, chainBuilder, handler);
            }

            return true;
        }

        /// <summary>
        ///     Create configuration from a single text value
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>The configuration</returns>
        protected abstract TConfigurationType CreateAndParseConfigurationFromDefaultValue(string text);

        /// <summary>
        /// Wire up a message handler from attribute
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        protected abstract bool WireUpFromAttribute<TMessageType, THandlerType>(
            TAttributeType attribute,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler);

        /// <summary>
        /// Wire up from configuration
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler type</typeparam>
        /// <param name="configuration">The configuration</param>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        protected abstract bool WireUpFromConfiguration<TMessageType, THandlerType>(
            TConfigurationType configuration,
            IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler);
    }
}