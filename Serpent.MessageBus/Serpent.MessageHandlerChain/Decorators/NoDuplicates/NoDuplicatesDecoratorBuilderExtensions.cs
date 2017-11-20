// ReSharper disable CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.NoDuplicates;

    /// <summary>
    ///     Provides extensions for the distinct decorator builder
    /// </summary>
    public static class NoDuplicatesDecoratorBuilderExtensions
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
        public static NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> KeySelector<TMessageType, TKeyType>(
            this NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> builder,
            Func<TMessageType, Task<TKeyType>> keySelector)
        {
            return builder.KeySelector((msg, token) => keySelector(msg));
        }
    }
}