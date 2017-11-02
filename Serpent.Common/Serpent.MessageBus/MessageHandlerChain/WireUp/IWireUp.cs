namespace Serpent.MessageBus.MessageHandlerChain.WireUp
{
    using System;

    /// <summary>
    /// The IWireUp interface
    /// </summary>
    public interface IWireUp
    {
        /// <summary>
        /// The type of the attribute used to identify the decorator
        /// </summary>
        Type AttributeType { get; }

        /// <summary>
        /// The type of the configuration used to identify the decorator
        /// </summary>
        Type ConfigurationType { get; }

        /// <summary>
        /// Creates a new configuration instance from a single text line
        /// </summary>
        /// <param name="text">The text to parse and create configuration from</param>
        /// <returns>The new configuration object</returns>
        object CreateConfigurationFromDefaultValue(string text);

        /// <summary>
        /// WireUp a message handler chain from an attribute
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        void WireUpFromAttribute<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler);

        /// <summary>
        /// WireUp a message handler chain from configuration
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler type</typeparam>
        /// <param name="configuration">The configuration</param>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        void WireUpFromConfiguration<TMessageType, THandlerType>(object configuration, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler);
    }
}