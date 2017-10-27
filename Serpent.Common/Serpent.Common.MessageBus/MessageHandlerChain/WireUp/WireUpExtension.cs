// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// Provides extensions to wire up decorators for a message handler
    /// </summary>
    public static class WireUpExtension
    {
        /// <summary>
        /// Wires up a message handler from attributes on a specified type
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TWireUpType">The type to get attributes from</typeparam>
        /// <typeparam name="THandlerType">The handler</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> WireUp<TMessageType, TWireUpType, THandlerType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            return WireUpMap.Default.WireUpHandlerFromAttributes<TMessageType, TWireUpType, THandlerType>(messageHandlerChainBuilder, handler);
        }

        /// <summary>
        /// Wires up a message handler from attributes on the message handler type
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> WireUp<TMessageType, THandlerType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            return WireUpMap.Default.WireUpHandlerFromAttributes<TMessageType, THandlerType, THandlerType>(messageHandlerChainBuilder, handler);
        }
    }
}