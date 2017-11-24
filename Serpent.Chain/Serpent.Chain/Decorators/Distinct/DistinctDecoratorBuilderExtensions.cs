// ReSharper disable CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Distinct;

    /// <summary>
    ///     Provides extensions for the distinct decorator builder
    /// </summary>
    public static class DistinctDecoratorBuilderExtensions
    {
        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <typeparam name="TKeyType">
        ///     The key type
        /// </typeparam>
        /// <param name="builder">
        ///     The decorator builder
        /// </param>
        /// <param name="keySelector">
        ///     The key selector
        /// </param>
        /// <returns>
        ///     A builder
        /// </returns>
        public static DistinctDecoratorBuilder<TMessageType, TKeyType> KeySelector<TMessageType, TKeyType>(
            this DistinctDecoratorBuilder<TMessageType, TKeyType> builder,
            Func<TMessageType, Task<TKeyType>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return builder.KeySelector((msg, token) => keySelector(msg));
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <typeparam name="TKeyType">
        ///     The key type
        /// </typeparam>
        /// <param name="builder">
        ///     The decorator builder
        /// </param>
        /// <param name="keySelector">
        ///     The key selector
        /// </param>
        /// <returns>
        ///     A builder
        /// </returns>
        public static DistinctDecoratorBuilder<TMessageType, TKeyType> KeySelector<TMessageType, TKeyType>(
            this DistinctDecoratorBuilder<TMessageType> builder,
            Func<TMessageType, Task<TKeyType>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return builder.KeySelector((msg, token) => keySelector(msg));
        }
    }
}