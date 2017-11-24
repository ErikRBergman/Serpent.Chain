// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Distinct;
    using Serpent.Chain.WireUp;

    /// <summary>
    ///     The distinct decorator extensions
    /// </summary>
    public static class DistinctExtensions
    {
        /// <summary>
        ///     Lets a message with the specified key through once
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="keySelector">The key selector</param>
        /// <returns>The builder</returns>
        [ExtensionMethodSelector(DistinctWireUp.WireUpExtensionName)]
        public static IChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, TKeyType> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return chainBuilder.AddDecorator(currentHandler => new DistinctDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        /// <summary>
        ///     Lets a message pass through once.
        ///     If the message is a value type (like int, or float) the value is for key.
        ///     If the message is a reference type (like a class or interface), the reference is used for key
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> Distinct<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.AddDecorator(currentHandler => new DistinctDecorator<TMessageType, TMessageType>(currentHandler, m => m));
        }

        /// <summary>
        ///     Lets a message with the specified key pass through once
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="config">The action used to configure the distinct decorator</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> Distinct<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Action<DistinctDecoratorBuilder<TMessageType>> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var builder = new DistinctDecoratorBuilder<TMessageType>();
            config(builder);
            return chainBuilder.AddDecorator(builder);
        }

        /// <summary>
        ///     Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="keySelector">The ASYNC key selector</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task<TKeyType>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return chainBuilder.AddDecorator(
                currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, (message, token) => keySelector(message)));
        }
    }
}