// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;

    using Serpent.Chain.Decorators.NoDuplicates;
    using Serpent.Chain.WireUp;

    /// <summary>
    ///     The no duplicates decorator extensions type
    /// </summary>
    public static class NoDuplicatesExtensions
    {
        /// <summary>
        ///     Prevents more than once identical message from being handled concurrently. Messages not handled are dropped.
        ///     If the message type is a value type, it's value is used.
        ///     If the message type is a reference type, that instance of the message can not be handled concurrently
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> NoDuplicates<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.AddDecorator(nextHandler => new NoDuplicatesDecorator<TMessageType, TMessageType>(nextHandler, m => m));
        }

        /// <summary>
        ///     Prevents more than a single message with the same key from being handled concurrently.
        ///     The keySelector defines the key. All messages identified as duplicates are dropped.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="keySelector">The key selector used to detemine the key</param>
        /// <returns>The message handler chain builder</returns>
        [ExtensionMethodSelector(NoDuplicatesWireUp.WireUpExtensionName)]
        public static IChainBuilder<TMessageType> NoDuplicates<TMessageType, TKeyType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, TKeyType> keySelector)
        {
            return chainBuilder.AddDecorator(nextHandler => new NoDuplicatesDecorator<TMessageType, TKeyType>(nextHandler, keySelector));
        }

        /// <summary>
        ///     Prevents more than a single message with the same key from being handled concurrently.
        ///     The keySelector defines the key. All messages identified as duplicates are dropped.
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="chainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="config">An action that configures the no duplicates decorator</param>
        /// <returns>
        ///     The message handler chain builder
        /// </returns>
        public static IChainBuilder<TMessageType> NoDuplicates<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Action<NoDuplicatesDecoratorBuilder<TMessageType>> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var builder = new NoDuplicatesDecoratorBuilder<TMessageType>();
            config(builder);
            return chainBuilder.AddDecorator(builder);
        }
    }
}