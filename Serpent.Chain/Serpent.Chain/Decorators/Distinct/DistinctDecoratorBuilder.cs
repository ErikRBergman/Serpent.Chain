// ReSharper disable StyleCop.SA1402
// ReSharper disable ParameterHidesMember

namespace Serpent.Chain.Decorators.Distinct
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Exceptions;

    /// <summary>
    ///     Provides a builder for the distinct decorator
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class DistinctDecoratorBuilder<TMessageType> : DecoratorBuilder<TMessageType>
    {
        private Func<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>> buildMethod;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DistinctDecoratorBuilder{TMessageType}" /> class.
        /// </summary>
        public DistinctDecoratorBuilder()
        {
            this.buildMethod = InternalBuildDecorator;
        }

        /// <summary>
        ///     Builds a new decorator
        /// </summary>
        /// <returns>The decorator</returns>
        public override Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator()
        {
            return this.buildMethod();
        }

        /// <summary>
        ///     Sets the key equality comparer
        /// </summary>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="equalityComparer">The key equality comparer</param>
        /// <returns>A builder</returns>
        public DistinctDecoratorBuilder<TMessageType, TKeyType> EqualityComparer<TKeyType>(IEqualityComparer<TKeyType> equalityComparer)
        {
            equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
            var builder = this.CreateKeyBuilder<TKeyType>().EqualityComparer(equalityComparer);
            return builder;
        }

        /// <summary>
        ///     Sets the key equality comparer
        /// </summary>
        /// <param name="equalityComparer">The key equality comparer</param>
        /// <returns>A builder</returns>
        public DistinctDecoratorBuilder<TMessageType, TMessageType> EqualityComparer(IEqualityComparer<TMessageType> equalityComparer)
        {
            equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
            var builder = this.CreateKeyBuilder<TMessageType>().EqualityComparer(equalityComparer);
            builder.KeySelector(m => m);

            return builder;
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public DistinctDecoratorBuilder<TMessageType, TKeyType> KeySelector<TKeyType>(Func<TMessageType, TKeyType> keySelector)
        {
            keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            return this.CreateKeyBuilder<TKeyType>().KeySelector(keySelector);
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public DistinctDecoratorBuilder<TMessageType, TKeyType> KeySelector<TKeyType>(Func<TMessageType, Task<TKeyType>> keySelector)
        {
            keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            return this.CreateKeyBuilder<TKeyType>().KeySelector(keySelector);
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public DistinctDecoratorBuilder<TMessageType, TKeyType> KeySelector<TKeyType>(Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector)
        {
            return this.CreateKeyBuilder<TKeyType>().KeySelector(keySelector);
        }

        private static Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> InternalBuildDecorator()
        {
            return innerHandler => new DistinctDecorator<TMessageType, TMessageType>(innerHandler, m => m).HandleMessageAsync;
        }

        private DistinctDecoratorBuilder<TMessageType, TKeyType> CreateKeyBuilder<TKeyType>()
        {
            var builder = new DistinctDecoratorBuilder<TMessageType, TKeyType>();
            this.buildMethod = builder.BuildDecorator;
            return builder;
        }
    }

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
    public class DistinctDecoratorBuilder<TMessageType, TKeyType> : DecoratorBuilder<TMessageType>
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
        public DistinctDecoratorBuilder<TMessageType, TKeyType> EqualityComparer(IEqualityComparer<TKeyType> equalityComparer)
        {
            this.equalityComparer = equalityComparer;
            return this;
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public DistinctDecoratorBuilder<TMessageType, TKeyType> KeySelector(Func<TMessageType, TKeyType> keySelector)
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
        public DistinctDecoratorBuilder<TMessageType, TKeyType> KeySelector(Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector)
        {
            keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.asyncKeySelector = keySelector;
            this.keySelector = null;
            return this;
        }
    }
}