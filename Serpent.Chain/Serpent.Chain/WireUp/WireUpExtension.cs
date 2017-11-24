// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System.Collections.Generic;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.WireUp;

    /// <summary>
    ///     Provides extensions to wire up decorators for a message handler
    /// </summary>
    public static class WireUpExtension
    {
        /// <summary>
        ///     Wires up a message handler from attributes on a specified type
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TWireUpType">The type to get attributes from</typeparam>
        /// <typeparam name="THandlerType">The handler</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> WireUp<TMessageType, TWireUpType, THandlerType>(
            this IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            return WireUpMap.Default.WireUpHandlerFromAttributes<TMessageType, TWireUpType, THandlerType>(chainBuilder, handler);
        }

        /// <summary>
        ///     Wires up a message handler from attributes on the message handler type
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The handler</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="handler">The handler</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> WireUp<TMessageType, THandlerType>(
            this IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            return WireUpMap.Default.WireUpHandlerFromAttributes<TMessageType, THandlerType, THandlerType>(chainBuilder, handler);
        }

        /// <summary>
        /// Wires up a message handler from configuration on the message handler type
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <typeparam name="THandlerType">
        /// The handler
        /// </typeparam>
        /// <param name="chainBuilder">
        /// The message handler chain builder
        /// </param>
        /// <param name="handler">
        /// The handler
        /// </param>
        /// <param name="wireUpConfigurationObjects">
        /// The wire Up Configuration Objects.
        /// </param>
        /// <returns>
        /// A message handler chain builder
        /// </returns>
        public static IChainBuilder<TMessageType> WireUp<TMessageType, THandlerType>(
            this IChainBuilder<TMessageType> chainBuilder,
            THandlerType handler,
            IEnumerable<object> wireUpConfigurationObjects)
            where THandlerType : IMessageHandler<TMessageType>
        {
            return WireUpMap.Default.WireUpHandlerFromConfiguration(chainBuilder, handler, wireUpConfigurationObjects);
        }

        /// <summary>
        /// Wires up a message handler from configuration on the message handler type
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="chainBuilder">
        /// The message handler chain builder
        /// </param>
        /// <param name="wireUpConfigurationObjects">
        /// The wire Up Configuration Objects.
        /// </param>
        /// <returns>
        /// A message handler chain builder
        /// </returns>
        public static IChainBuilder<TMessageType> WireUp<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            IEnumerable<object> wireUpConfigurationObjects)
        {
            return WireUpMap.Default.WireUpHandlerFromConfiguration(chainBuilder, (IMessageHandler<TMessageType>)null, wireUpConfigurationObjects);
        }
    }
}