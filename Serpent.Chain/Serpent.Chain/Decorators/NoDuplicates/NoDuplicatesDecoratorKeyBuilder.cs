// ReSharper disable StyleCop.SA1402
// ReSharper disable ParameterHidesMember

namespace Serpent.Chain.Decorators.NoDuplicates
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Exceptions;

    /// <summary>
    ///     The distinct decorator builder with key selector
    /// </summary>
    /// <typeparam name="TMessageType">
    ///     The message type
    /// </typeparam>
    /// <typeparam name="TKeyType">
    ///     The key type
    /// </typeparam>
    /// <returns>
    ///     A builder
    /// </returns>
    public class NoDuplicatesDecoratorKeyBuilder<TMessageType, TKeyType> : DecoratorBuilder<TMessageType>
    {
        private IEqualityComparer<TKeyType> equalityComparer;

        private Func<TMessageType, TKeyType> keySelector;

        /// <inheritdoc />
        public override Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator()
        {
            if (this.keySelector == null)
            {
                throw new KeySelectorMissingException("KeySelector not set and it can not be inferred from equality comparer");
            }

            return innerHandler => new NoDuplicatesDecorator<TMessageType, TKeyType>(innerHandler, this.keySelector, this.equalityComparer).HandleMessageAsync;
        }

        /// <summary>
        ///     Sets the key equality comparer
        /// </summary>
        /// <param name="equalityComparer">The key equality comparer</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorKeyBuilder<TMessageType, TKeyType> EqualityComparer(IEqualityComparer<TKeyType> equalityComparer)
        {
            this.equalityComparer = equalityComparer;
            return this;
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorKeyBuilder<TMessageType, TKeyType> KeySelector(Func<TMessageType, TKeyType> keySelector)
        {
            keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.keySelector = keySelector;
            return this;
        }
    }
}