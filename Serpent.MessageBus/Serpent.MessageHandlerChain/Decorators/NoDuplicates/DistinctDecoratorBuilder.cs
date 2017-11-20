// ReSharper disable StyleCop.SA1402
// ReSharper disable ParameterHidesMember

namespace Serpent.MessageHandlerChain.Decorators.NoDuplicates
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.Distinct;
    using Serpent.MessageHandlerChain.Exceptions;

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
    public class NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> : DecoratorBuilder<TMessageType>
    {
        private Func<TMessageType, CancellationToken, Task<TKeyType>> asyncKeySelector;

        private IEqualityComparer<TKeyType> equalityComparer;

        private Func<TMessageType, TKeyType> keySelector;

        /// <inheritdoc />
        public override Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator()
        {
            if (this.asyncKeySelector != null)
            {
                return innerHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(innerHandler, this.asyncKeySelector, this.equalityComparer).HandleMessageAsync;
            }

            if (this.keySelector == null)
            {
                throw new KeySelectorMissingException("KeySelector not set and it can not be inferred from equality comparer");
            }

            return innerHandler => new DistinctDecorator<TMessageType, TKeyType>(innerHandler, this.keySelector, this.equalityComparer).HandleMessageAsync;
        }

        /// <summary>
        ///     Sets the key equality comparer
        /// </summary>
        /// <param name="equalityComparer">The key equality comparer</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> EqualityComparer(IEqualityComparer<TKeyType> equalityComparer)
        {
            this.equalityComparer = equalityComparer;
            return this;
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> KeySelector(Func<TMessageType, TKeyType> keySelector)
        {
            keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.keySelector = keySelector;
            this.asyncKeySelector = null;
            return this;
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> KeySelector(Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector)
        {
            keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.asyncKeySelector = keySelector;
            this.keySelector = null;
            return this;
        }
    }
}